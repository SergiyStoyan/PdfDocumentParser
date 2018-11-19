## PdfDocumentParser

PdfDocumentParser is a parsing engine intended to extract of text/images from PDF documents that conform to a recognizable graphic layout - such as invoices and the like. The main parsing approach is based on finding certain text or image fragments in page and then extracting text/images located relatively to those fragments.

Within this scope PdfDocumentParser is capable of the following:
- operating with text represented by PDF entities (meant for native PDF files);
- processing OCR'ed text (meant for scanned PDF files);
- image search/comparison/extraction (meant for either native or scanned PDF files);

PdfDocumentParser was designed to be incorporated into custom applications hopefully without need of change.

PdfDocumentParser API consists of:
- Template Editor where parsing templates can be created or debugged in an easy manner;
- Parsing API that allows custom applications to parse PDF files in a custom manner with little effort required;

PdfDocumentParser is a .NET DLL.

[More details...](https://sergeystoyan.github.io/PdfDocumentParser/)

## InvoiceParser
[Invoice Parser](https://github.com/sergeystoyan/PdfDocumentParser/tree/lib%2Bcustomization/InvoiceParser) is a custom desktop application based on [PdfDocumentParser](https://github.com/sergeystoyan/PdfDocumentParser). While it was developed for custom needs, it can be used as a framework or example of incorporating PdfDocumentParser into a custom code.

[More details...](https://sergeystoyan.github.io/PdfDocumentParser/#6)

## Support
Contact me if you need to solve a parsing task of any complexity.

