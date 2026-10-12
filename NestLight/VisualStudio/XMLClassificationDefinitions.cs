using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;

namespace NestLight.VisualStudio
{
    internal static class XMLClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.XmlTag)]
        internal static ClassificationTypeDefinition Tag;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.XmlDelimiter)]
        internal static ClassificationTypeDefinition Delimiter;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.XmlAttribute)]
        internal static ClassificationTypeDefinition Attribute;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.XmlValue)]
        internal static ClassificationTypeDefinition AttributeValue;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.XmlComment)]
        internal static ClassificationTypeDefinition Comment;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.XmlCData)]
        internal static ClassificationTypeDefinition CData;
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.XmlTag)]
    [Name("Template XML Tag Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class XMLTagFormat : TplFormat { public XMLTagFormat() : base("Template XML Tag", 0x4A, 0x90, 0xD9) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.XmlDelimiter)]
    [Name("Template XML Delimiter Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class XMLDelimiterFormat : TplFormat { public XMLDelimiterFormat() : base("Template XML Delimiter", 0x90, 0x90, 0x90) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.XmlAttribute)]
    [Name("Template XML Attribute Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class XMLAttributeFormat : TplFormat { public XMLAttributeFormat() : base("Template XML Attribute", 0x56, 0xB0, 0xC8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.XmlValue)]
    [Name("Template XML Attribute Value Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class XMLAttributeValueFormat : TplFormat { public XMLAttributeValueFormat() : base("Template XML Attribute Value", 0xCE, 0x91, 0x78) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.XmlComment)]
    [Name("Template XML Comment Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class XMLCommentFormat : TplFormat { public XMLCommentFormat() : base("Template XML Comment", 0x6A, 0x99, 0x55, italic: true) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.XmlCData)]
    [Name("Template XML CDATA Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class XMLCDataFormat : TplFormat { public XMLCDataFormat() : base("Template XML CDATA", 0xB5, 0x9A, 0x6A) { } }
}
