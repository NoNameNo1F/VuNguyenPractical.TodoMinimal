using FastEndpoints;

namespace TodoMinimal.API.Groups
{
    public class NoteGroup : Group
    {
        public NoteGroup()
        {
            Configure("notes", endpoint =>
            {
                endpoint.Description(x =>
                {
                    x.WithTags("Notes");
                    x.ProducesProblemFE<InternalErrorResponse>(500);
                });
            });
        }
    }
}