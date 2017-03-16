using Voxteneo.Core.Domains;

namespace Learning.Entities
{
    public class Vehicle : BaseModifiy
    {
        public string Name { get; set; }
        public VehicleModel VehicleModel { get; set; }
    }
}