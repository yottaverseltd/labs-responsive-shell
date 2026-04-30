namespace LabsResponsiveShell.Data;

public static class AvatarPalette
{
    public static int IndexFor(string seed)
    {
        unchecked
        {
            uint hash = 2166136261;
            foreach (var ch in seed)
            {
                hash ^= ch;
                hash *= 16777619;
            }

            return (int)(hash % 6);
        }
    }
}
