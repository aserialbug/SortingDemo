namespace sort.Application
{
    public record Options
    {
        public string TemporaryDirectory { get; init; }
        public bool ShouldDeleteTemporaryFiles { get; init; }
    }
}