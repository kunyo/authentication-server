namespace AuthenticationService
{
    public class ValidateResourceOwnerGrantResponse
    {
        public static ValidateResourceOwnerGrantResponse Invalid = new ValidateResourceOwnerGrantResponse(false, null);

        public ValidateResourceOwnerGrantResponse(bool isValid, string subjectId)
        {
            IsValid = isValid;
            SubjectId = subjectId;
        }

        public bool IsValid { get; }
        public string SubjectId { get; }
    }
}
