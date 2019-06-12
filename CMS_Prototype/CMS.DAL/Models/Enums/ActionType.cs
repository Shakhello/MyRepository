namespace CMS.DAL.Models
{
    public enum ActionType
    {
        Goto = 0,
        CreateDocument = 1,
        UpdateDocument = 2,
        UpdateField = 3,
        Search = 4,
        GoBack = 6,
        SearchTicketsWithAutoComplete = 7,
        ServiceCall = 8,
        DeleteDocument = 9
    }
}
