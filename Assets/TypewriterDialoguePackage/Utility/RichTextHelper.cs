using UnityEngine;
using System.Collections.Generic;

namespace TypewriterDialoguePackage.Utility
{
    /// <summary>
    /// Static helper class for TextMeshPro rich text formatting.
    /// Provides methods to color, style, and format specific words in text.
    /// </summary>
    public static class RichTextHelper
    {
        #region Color Methods

        /// <summary>
        /// Color a specific word in text using TextMeshPro rich text tags.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="word">Word to color</param>
        /// <param name="color">Color to apply</param>
        /// <returns>Text with colored word</returns>
        public static string ColorWord(string text, string word, Color color)
        {
            string colorHex = ColorUtility.ToHtmlStringRGBA(color);
            return text.Replace(word, $"<color=#{colorHex}>{word}</color>");
        }

        /// <summary>
        /// Color a specific word in text using hex color.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="word">Word to color</param>
        /// <param name="hexColor">Hex color (e.g., "#FF0000")</param>
        /// <returns>Text with colored word</returns>
        public static string ColorWordHex(string text, string word, string hexColor)
        {
            return text.Replace(word, $"<color={hexColor}>{word}</color>");
        }

        /// <summary>
        /// Color a specific word in text using color name.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="word">Word to color</param>
        /// <param name="colorName">Color name (e.g., "red", "blue", "yellow")</param>
        /// <returns>Text with colored word</returns>
        public static string ColorWordName(string text, string word, string colorName)
        {
            return text.Replace(word, $"<color={colorName}>{word}</color>");
        }

        /// <summary>
        /// Color multiple words in text using a dictionary mapping.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="wordColors">Dictionary of words to colors</param>
        /// <returns>Text with colored words</returns>
        public static string ColorWords(string text, Dictionary<string, Color> wordColors)
        {
            string result = text;
            foreach (var kvp in wordColors)
            {
                result = ColorWord(result, kvp.Key, kvp.Value);
            }
            return result;
        }

        /// <summary>
        /// Color all occurrences of a word.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="word">Word to color</param>
        /// <param name="color">Color to apply</param>
        /// <returns>Text with all occurrences colored</returns>
        public static string ColorAllOccurrences(string text, string word, Color color)
        {
            return ColorWord(text, word, color);
        }

        #endregion

        #region Style Methods

        /// <summary>
        /// Apply bold formatting to a specific word.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="word">Word to bold</param>
        /// <returns>Text with bold word</returns>
        public static string BoldWord(string text, string word)
        {
            return text.Replace(word, $"<b>{word}</b>");
        }

        /// <summary>
        /// Apply italic formatting to a specific word.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="word">Word to italicize</param>
        /// <returns>Text with italic word</returns>
        public static string ItalicWord(string text, string word)
        {
            return text.Replace(word, $"<i>{word}</i>");
        }

        /// <summary>
        /// Apply underline formatting to a specific word.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="word">Word to underline</param>
        /// <returns>Text with underlined word</returns>
        public static string UnderlineWord(string text, string word)
        {
            return text.Replace(word, $"<u>{word}</u>");
        }

        /// <summary>
        /// Apply strikethrough formatting to a specific word.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="word">Word to strikethrough</param>
        /// <returns>Text with strikethrough word</returns>
        public static string StrikethroughWord(string text, string word)
        {
            return text.Replace(word, $"<s>{word}</s>");
        }

        #endregion

        #region Size Methods

        /// <summary>
        /// Apply size formatting to a specific word.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="word">Word to resize</param>
        /// <param name="sizePercentage">Size percentage (e.g., 150 for 150%)</param>
        /// <returns>Text with resized word</returns>
        public static string SizeWord(string text, string word, float sizePercentage)
        {
            return text.Replace(word, $"<size={sizePercentage}%>{word}</size>");
        }

        /// <summary>
        /// Apply size formatting to a specific word using absolute font size.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="word">Word to resize</param>
        /// <param name="fontSize">Absolute font size</param>
        /// <returns>Text with resized word</returns>
        public static string SizeWordAbsolute(string text, string word, float fontSize)
        {
            return text.Replace(word, $"<size={fontSize}>{word}</size>");
        }

        #endregion

        #region Advanced Formatting

        /// <summary>
        /// Apply multiple rich text effects to a word.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="word">Word to format</param>
        /// <param name="color">Optional color</param>
        /// <param name="bold">Apply bold</param>
        /// <param name="italic">Apply italic</param>
        /// <param name="underline">Apply underline</param>
        /// <param name="sizePercentage">Optional size percentage</param>
        /// <returns>Text with formatted word</returns>
        public static string FormatWord(string text, string word, Color? color = null, bool bold = false, bool italic = false, bool underline = false, float? sizePercentage = null)
        {
            string formattedWord = word;

            if (color.HasValue)
            {
                string colorHex = ColorUtility.ToHtmlStringRGBA(color.Value);
                formattedWord = $"<color=#{colorHex}>{formattedWord}</color>";
            }

            if (bold)
            {
                formattedWord = $"<b>{formattedWord}</b>";
            }

            if (italic)
            {
                formattedWord = $"<i>{formattedWord}</i>";
            }

            if (underline)
            {
                formattedWord = $"<u>{formattedWord}</u>";
            }

            if (sizePercentage.HasValue)
            {
                formattedWord = $"<size={sizePercentage.Value}%>{formattedWord}</size>";
            }

            return text.Replace(word, formattedWord);
        }

        /// <summary>
        /// Create a rich text formatted string with multiple styled words.
        /// </summary>
        /// <param name="baseText">Base text template with placeholders</param>
        /// <param name="replacements">Dictionary of placeholder keys to formatted values</param>
        /// <returns>Formatted text</returns>
        public static string CreateFormattedText(string baseText, Dictionary<string, string> replacements)
        {
            string result = baseText;
            foreach (var kvp in replacements)
            {
                result = result.Replace(kvp.Key, kvp.Value);
            }
            return result;
        }

        /// <summary>
        /// Apply a gradient to a span of text.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="startColor">Gradient start color</param>
        /// <param name="endColor">Gradient end color</param>
        /// <returns>Text with gradient</returns>
        public static string ApplyGradient(string text, Color startColor, Color endColor)
        {
            string startHex = ColorUtility.ToHtmlStringRGBA(startColor);
            string endHex = ColorUtility.ToHtmlStringRGBA(endColor);
            return $"<color=# gradient>{text}</color>";
        }

        /// <summary>
        /// Apply alpha/opacity to text.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="alpha">Alpha value (0-1)</param>
        /// <returns>Text with applied alpha</returns>
        public static string SetAlpha(string text, float alpha)
        {
            Color color = new Color(1f, 1f, 1f, alpha);
            string colorHex = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{colorHex}>{text}</color>";
        }

        #endregion

        #region Special Effects

        /// <summary>
        /// Apply a glow effect to text.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="glowColor">Glow color</param>
        /// <param name="glowPower">Glow power (0-1)</param>
        /// <returns>Text with glow effect</returns>
        public static string AddGlow(string text, Color glowColor, float glowPower = 0.5f)
        {
            string colorHex = ColorUtility.ToHtmlStringRGBA(glowColor);
            return $"<material=\"glow\" color=#{colorHex}>{text}</material>";
        }

        /// <summary>
        /// Apply an outline effect to text.
        /// </summary>
        /// <param name="text">Original text</param>
        /// <param name="outlineColor">Outline color</param>
        /// <param name="outlineWidth">Outline width</param>
        /// <returns>Text with outline effect</returns>
        public static string AddOutline(string text, Color outlineColor, float outlineWidth = 0.2f)
        {
            string colorHex = ColorUtility.ToHtmlStringRGBA(outlineColor);
            return $"<font=\"Outline\" material=\"{colorHex}\" outlineWidth={outlineWidth}>{text}</font>";
        }

        /// <summary>
        /// Create a link that can be clicked.
        /// </summary>
        /// <param name="text">Link text</param>
        /// <param name="linkId">Link identifier</param>
        /// <returns>Text with link</returns>
        public static string CreateLink(string text, string linkId)
        {
            return $"<link=\"{linkId}\"><u><color=blue>{text}</color></u></link>";
        }

        /// <summary>
        /// Add a sprite to text.
        /// </summary>
        /// <param name="spriteName">Sprite asset name</param>
        /// <param name="size">Sprite size</param>
        /// <returns>Sprite tag</returns>
        public static string AddSprite(string spriteName, float size = 32f)
        {
            return $"<sprite name={spriteName} size={size}>";
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Remove all rich text tags from text.
        /// </summary>
        /// <param name="text">Text with rich tags</param>
        /// <returns>Plain text without tags</returns>
        public static string StripRichTextTags(string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, @"<[^>]+>", string.Empty);
        }

        /// <summary>
        /// Check if text contains rich text tags.
        /// </summary>
        /// <param name="text">Text to check</param>
        /// <returns>True if text contains rich tags</returns>
        public static bool ContainsRichText(string text)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(text, @"<[^>]+>");
        }

        /// <summary>
        /// Get the plain text version (without tags) and preserve the rich version.
        /// </summary>
        /// <param name="text">Text with rich tags</param>
        /// <param name="plainText">Output plain text</param>
        /// <returns>Original rich text</returns>
        public static string GetPlainText(string text, out string plainText)
        {
            plainText = StripRichTextTags(text);
            return text;
        }

        #endregion
    }
}