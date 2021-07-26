using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _mapper;

        public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetHotels()
        {
            var Hotels =await _unitOfWork.Hotels.GetAll();
            var result = _mapper.Map<IList<HotelDTO>>(Hotels);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("{id:int}",Name = "GetHotel")]
        public async Task<IActionResult> GetHotel(int id)
        {
            var Hotel = await _unitOfWork.Hotels.Get(q=>q.Id==id, new List<string> { "Country" });
            var result = _mapper.Map<HotelDTO>(Hotel);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO DTO)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var hotel = _mapper.Map<Hotel>(DTO);
                await _unitOfWork.Hotels.Insert(hotel);
                await _unitOfWork.Save(); 
                return CreatedAtRoute("GetHotel", new { id = hotel.Id });

            }
            catch (Exception ex)
            {

                _logger.LogError(ex,$"something Went wrong");
                return StatusCode(500, "Internal mnyka Error");

            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateHotel(int id,[FromBody] CreateHotelDTO DTO)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var hotel =  await _unitOfWork.Hotels.Get(o=>o.Id==id);
                if(hotel==null)
                {
                    _logger.LogError( $"hotel not Found to edit ");
                    return StatusCode(500, "Internal mnyka Error");


                }
                _mapper.Map(DTO, hotel);
                _unitOfWork.Hotels.Update(hotel);

                await _unitOfWork.Save();
                return NoContent();

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"something Went wrong");
                return StatusCode(500, "Internal mnyka Error");

            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteHotel(int id)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var hotel = await _unitOfWork.Hotels.Get(o => o.Id == id);
                if (hotel == null)
                {
                    _logger.LogError($"hotel not Found to edit ");
                    return StatusCode(500, "Internal mnyka Error");


                }
                
               await _unitOfWork.Hotels.Delete(id);

                await _unitOfWork.Save();
                return NoContent();

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"something Went wrong");
                return StatusCode(500, "Internal mnyka Error");

            }
        }


    }

}
