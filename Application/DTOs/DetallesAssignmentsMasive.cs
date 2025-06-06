using ALFINapp.API.DTOs;
using ALFINapp.Domain.Entities;

namespace ALFINapp.Application.DTOs
{
    public class DetallesAssignmentsMasive
    {
        public List<DetallesAssignmentsSupervisor> SupervisoresConClientes { get; set; } = new List<DetallesAssignmentsSupervisor>();
        public DetallesAssignmentsMasive(List<DtoVAsignarClientesSupervisores> dtoView)
        {
            if (dtoView != null && dtoView.Count > 0)
            {
                var groupedBySupervisor = dtoView
                    .GroupBy(x => x.dni_supervisor)
                    .Select(g => new DetallesAssignmentsSupervisor(g.Key, g.Select(c => new Cliente
                    {
                        Dni = c.dni_cliente,
                        Telefonos = new List<string>
                        {
                            !string.IsNullOrWhiteSpace(c.telefono_1) && c.telefono_1 != "NULL"
                                ? c.telefono_1
                                : string.Empty,
                            !string.IsNullOrWhiteSpace(c.telefono_2) && c.telefono_2 != "NULL"
                                ? c.telefono_2
                                : string.Empty,
                            !string.IsNullOrWhiteSpace(c.telefono_3) && c.telefono_3 != "NULL"
                                ? c.telefono_3
                                : string.Empty,
                            !string.IsNullOrWhiteSpace(c.telefono_4) && c.telefono_4 != "NULL"
                                ? c.telefono_4
                                : string.Empty,
                            !string.IsNullOrWhiteSpace(c.telefono_5) && c.telefono_5 != "NULL"
                                ? c.telefono_5
                                : string.Empty
                        },
                        FuenteBase = c.d_base
                    }).ToList()))
                    .ToList();

                SupervisoresConClientes.AddRange(groupedBySupervisor);
            }
        }
    }
    public class DetallesAssignmentsSupervisor
    {
        public int? IdSupervisor { get; set; } = 0;
        public string? DniSupervisor { get; set; } = String.Empty;
        public List<Cliente> Clientes { get; set; } = new List<Cliente>();
        public string? NombreLista { get; set; } = String.Empty;
        public DetallesAssignmentsSupervisor(string? dniSupervisor, List<Cliente> clientes)
        {
            DniSupervisor = dniSupervisor;
            Clientes = clientes;
        }
        public DetallesAssignmentsSupervisor()
        {
            DniSupervisor = String.Empty;
            Clientes = new List<Cliente>();
        }
    }
}