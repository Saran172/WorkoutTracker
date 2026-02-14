using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ReqDTO;
using WTAlgoServices.InitializeService;
using WTAlgoServices.Wservices;

namespace WorkoutTracker.API.Controllers
{
    [Route("api/[controller]/v{version:apiVersion}")]
    [Asp.Versioning.ApiVersion("2.0")]
    [Authorize]
    [ApiController]
    public class WTAlgorithmController : ControllerBase
    {
        private readonly WorkoutService _service;
        private readonly WorkoutInitializationService _workoutInitService;

        public WTAlgorithmController(WorkoutService service, WorkoutInitializationService workoutInitialization)
        {
            _service = service;
            _workoutInitService = workoutInitialization;
        }
        [HttpPost("initialize")]
        public IActionResult InitializeWorkout([FromBody] InitializeWorkoutRequest request)
        {
            var result = _workoutInitService.Initialize(request.User, request.ExerciseDefinitions, request.ExerciseHistory);
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("process")]
        public IActionResult ProcessWorkout(ProcessWorkoutRequest request)
        {
            var result = _service.ProcessExercise(request.TrainingGoal, request.Exercise, request.Session, request.Progress, request.VolumeMap);
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
