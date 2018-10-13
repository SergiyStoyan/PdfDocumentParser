## PdfDocumentParser

PdfDocumentParser is a parsing engine designed to extract text/images from PDF documents conforming to a predefined graphic layout - such as invoices and the like. The main approach of parsing is based on finding certain text or image fragments in page and then extracting text/images located relatively to those fragments.

Within the scope PdfDocumentParser implements the following options:
- PDF entities processing (the main way to operate with text in PdfDocumentParser);
- OCR processing (intended for scanned documents);
- Image processing (used either in native and scanned PDF files);

PdfDocumentParser was developed as a set of parsing tools that can be incorporated into a custom application hopefully without need of change. It provides:
- Template Editor where a PDF file can be open and a parsing template created or debugged;
- parsing engine API that can be called from a custom application;

### Assumptions
- A PDF file can consist of more than one document (e.g. multiple invoices).
- A document (e.g. invoice) can consist of more than one page.

### InvoiceParser
[Invoice Parser](https://github.com/sergeystoyan/PdfDocumentParser/tree/lib%2Bcustomization/InvoiceParser) is a custom desktop application based on the leverages provided by [PdfDocumentParser](https://github.com/sergeystoyan/PdfDocumentParser) and developed for certain needs. Most likely it will not fit your needs from scratch but can be used as a sample.

### Support
I can be contacted through github if you want me to upgrade PdfDocumentParser or develop a custom application based on it.

[More details...](https://sergeystoyan.github.io/PdfDocumentParser/)
