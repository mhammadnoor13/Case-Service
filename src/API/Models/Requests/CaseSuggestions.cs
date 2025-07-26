namespace API.Models.Requests
{
    public record SuggestionDto(string text);
    public record CaseSuggestionsDto(List<SuggestionDto> suggestions);
}
