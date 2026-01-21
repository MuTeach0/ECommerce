namespace ECommerce.Application.Common.Interfaces;
public static class UtilityService
{
    public static string MaskEmail(string email)
    {
        // 1. Validate input for null, empty, or whitespace
        if (string.IsNullOrWhiteSpace(email))
        {
            return email;
        }
        
        int atIndex = email.IndexOf('@');

        // Return original if '@' is missing or is the first character
        if (atIndex <= 0)
        {
            return email;
        }

        // Use Span to slice the string without creating new string objects in memory
        ReadOnlySpan<char> emailSpan = email.AsSpan();
        ReadOnlySpan<char> localPart = emailSpan[..atIndex];
        ReadOnlySpan<char> domainPart = emailSpan[atIndex..]; // Includes '@' and domain

        // 2. Apply masking logic based on local part length
        if (localPart.Length <= 2)
        {
            // For short local parts, mask all characters
            return new string('*', localPart.Length) + domainPart.ToString();
        }

        // For longer local parts, keep first and last characters, mask the middle
        char firstChar = localPart[0];
        char lastChar = localPart[^1];
        int middleLength = localPart.Length - 2;

        // Concatenate parts efficiently
        return string.Concat(firstChar.ToString(), new string('*', middleLength), lastChar, domainPart.ToString());
    }
}