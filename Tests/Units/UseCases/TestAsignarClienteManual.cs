using ALFINapp.Application.Interfaces.Asignacion;
using ALFINapp.Application.UseCases.Asignacion;
using ALFINapp.Domain.Interfaces;
using Moq;
using Xunit;

namespace ALFINapp.Tests.UseCases.Asignacion
{
    public class UseCaseAsignarClienteManualTest
    {
        private readonly Mock<IRepositoryAsignacion> _mockRepository;
        private readonly IUseCaseAsignarClienteManual _useCase;
        public UseCaseAsignarClienteManualTest()
        {
            _mockRepository = new Mock<IRepositoryAsignacion>();
            _useCase = new UseCaseAsignarClienteManual(_mockRepository.Object);
        }

        [Fact]
        public async Task Exec_WhenRepositoryReturnsSuccess_ReturnsSuccessResult()
        {
            string dniCliente = "12345678";
            int idUsuarioV = 2108;
            string baseTipo = "BDA365";
            
            _mockRepository.Setup(repo => repo.AsignarClienteManual(dniCliente, baseTipo, idUsuarioV))
                .ReturnsAsync((true, "Operation successful"));

            var result = await _useCase.exec(dniCliente, idUsuarioV, baseTipo);

            Assert.True(result.success);
            Assert.Equal("Asignacion exitosa", result.message);
            _mockRepository.Verify(repo => repo.AsignarClienteManual(dniCliente, baseTipo, idUsuarioV), Times.Once);
        }

        [Fact]
        public async Task Exec_WhenRepositoryReturnsFailure_ReturnsFailureResult()
        {
            string dniCliente = "12345678";
            int idUsuarioV = 1;
            string baseTipo = "TIPO1";
            string errorMessage = "Cliente no encontrado";
            
            _mockRepository.Setup(repo => repo.AsignarClienteManual(dniCliente, baseTipo, idUsuarioV))
                .ReturnsAsync((false, errorMessage));

            var result = await _useCase.exec(dniCliente, idUsuarioV, baseTipo);

            Assert.False(result.success);
            Assert.Equal(errorMessage, result.message);
            _mockRepository.Verify(repo => repo.AsignarClienteManual(dniCliente, baseTipo, idUsuarioV), Times.Once);
        }

        [Fact]
        public async Task Exec_WhenSqlExceptionOccurs_ReturnsFailureWithSqlMessage()
        {
            string dniCliente = "12345678";
            int idUsuarioV = 1;
            string baseTipo = "TIPO1";
            string errorMessage = "Error de SQL";
            
            var sqlError = new Exception("Error de SQL");
            
            var errorCollection = new { sqlError };
            var sqlException = new Exception("Error de SQL");
            
            _mockRepository.Setup(repo => repo.AsignarClienteManual(dniCliente, baseTipo, idUsuarioV))
                .ThrowsAsync(sqlException);

            var result = await _useCase.exec(dniCliente, idUsuarioV, baseTipo);

            Assert.False(result.success);
            Assert.Equal(errorMessage, result.message);
            _mockRepository.Verify(repo => repo.AsignarClienteManual(dniCliente, baseTipo, idUsuarioV), Times.Once);
        }

        [Fact]
        public async Task Exec_WhenGeneralExceptionOccurs_ReturnsFailureWithGenericMessage()
        {
            string dniCliente = "12345678";
            int idUsuarioV = 1;
            string baseTipo = "TIPO1";
            var exception = new Exception("Error general");
            
            _mockRepository.Setup(repo => repo.AsignarClienteManual(dniCliente, baseTipo, idUsuarioV))
                .ThrowsAsync(exception);

            var result = await _useCase.exec(dniCliente, idUsuarioV, baseTipo);

            Assert.False(result.success);
            Assert.Equal("Ocurrió un error inesperado en la asignación manual de cliente.", result.message);
            _mockRepository.Verify(repo => repo.AsignarClienteManual(dniCliente, baseTipo, idUsuarioV), Times.Once);
        }
    }
}