namespace TWLibrary
{
    public class TroupMovement
    {
        public string TargetId { get; set; }

        public string Type { get; set; }

        public string MovementId { get; set; }

        public override string ToString()
        {
            return $"TargetId: {TargetId}, Type: {Type}, MovementId {MovementId}";
        }
    }
}