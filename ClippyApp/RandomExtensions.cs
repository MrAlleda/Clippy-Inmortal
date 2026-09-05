namespace ClippyApp;

static class RandomExtensions
{
    public static T Pick<T>(this Random rng, IReadOnlyList<T> items) => items[rng.Next(items.Count)];
}
