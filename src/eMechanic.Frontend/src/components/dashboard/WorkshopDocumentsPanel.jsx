import React, { useState } from "react";
import {
  Box,
  Flex,
  HStack,
  Button,
  Heading,
  SimpleGrid,
  VStack,
  Text,
  Icon,
  Badge,
  Separator,
  Center,
  Input,
  DialogRoot,
  DialogBackdrop,
  DialogContent,
  DialogHeader,
  DialogBody,
  DialogFooter,
  DialogTitle,
  DialogCloseTrigger,
  DialogActionTrigger,
  Field,
  NativeSelect,
  Portal,
  DialogPositioner,
} from "@chakra-ui/react";
import { FileText, Plus, Trash2, Download, ShieldCheck, FileSpreadsheet, Image } from "lucide-react";
import { toaster } from "@/components/ui/toaster";

export const WorkshopDocumentsPanel = ({
  documents,
  loading,
  onUploadDocument,
  onDeleteDocument,
  onDownloadDocument,
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const [docName, setDocName] = useState("");
  const [docType, setDocType] = useState("Logo");
  const [file, setFile] = useState(null);

  const getDocTypeDetails = (type) => {
    switch (type) {
      case "Logo":
      case "logo":
        return { label: "Logo", color: "orange", icon: Image };
      case "GalleryImage":
      case "galleryImage":
        return { label: "Image from the gallery", color: "pink", icon: Image };
      case "Certificate":
        return { label: "Certificate", color: "green", icon: ShieldCheck };
      case "Invoice":
        return { label: "Invoice", color: "blue", icon: FileSpreadsheet };
      case "Insurance":
        return { label: "Insurance", color: "purple", icon: FileText };
      default:
        return { label: "Other", color: "gray", icon: FileText };
    }
  };

  const handleFileChange = (e) => {
    if (e.target.files && e.target.files[0]) {
      const selectedFile = e.target.files[0];
      setFile(selectedFile);
      if (!docName) {
        // Auto-fill document name
        setDocName(selectedFile.name.split(".")[0]);
      }
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!docName.trim()) {
      toaster.create({ title: "Błąd", description: "Document name is required", type: "error" });
      return;
    }
    
    // Simulate upload
    const mockDoc = {
      id: "doc-" + Date.now(),
      documentId: "doc-id-" + Date.now(),
      originalFileName: file ? file.name : `${docName}.pdf`,
      contentType: file ? file.type : "application/pdf",
      fileSize: file ? file.size : 1024 * 342, // Default mock size 342KB
      createdAt: new Date().toISOString(),
      type: docType,
      displayName: docName,
    };

    onUploadDocument(mockDoc, file);
    setIsOpen(false);
    setDocName("");
    setDocType("Certificate");
    setFile(null);
  };

  const formatBytes = (bytes) => {
    if (!bytes || isNaN(bytes)) return "";
    if (bytes === 0) return "0 Bytes";
    const k = 1024;
    const sizes = ["Bytes", "KB", "MB"];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + " " + sizes[i];
  };

  return (
    <VStack align="stretch" gap={6}>
      <Flex justify="space-between" align="center" wrap="wrap" gap={4}>
        <Box>
          <Heading size="2xl" fontWeight="black" tracking="tight" _dark={{ color: "white" }}>
            Workshop Documents
          </Heading>
          <Text color="gray.500" _dark={{ color: "gray.400" }} fontSize="md" mt={1}>
            Keep your certificates, insurance policies, and financial documents in a safe place.
          </Text>
        </Box>
        <Button
          colorPalette="orange"
          onClick={() => setIsOpen(true)}
          rounded="xl"
          gap={2}
          fontWeight="semibold"
          shadow="md"
        >
          <Icon as={Plus} boxSize={4} />
          Add a document
        </Button>
      </Flex>

      {documents.length > 0 ? (
        <SimpleGrid columns={{ base: 1, md: 2, lg: 3 }} gap={5}>
          {documents.map((doc) => {
            const docTypeInfo = getDocTypeDetails(doc.type || "Certificate");
            const DocIcon = docTypeInfo.icon;

            return (
              <Box
                key={doc.id}
                p={5}
                bg="white"
                _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
                borderWidth="1px"
                borderColor="gray.200"
                rounded="2xl"
                boxShadow="0 4px 20px -2px rgba(249, 115, 22, 0.02), 0 2px 8px -1px rgba(249, 115, 22, 0.02)"
                display="flex"
                flexDirection="column"
                justifyContent="space-between"
                gap={4}
                transition="all 0.2s ease"
                _hover={{
                  transform: "translateY(-4px)",
                  boxShadow: "0 12px 24px -10px rgba(249, 115, 22, 0.1), 0 6px 12px -5px rgba(249, 115, 22, 0.05)",
                  borderColor: "orange.300",
                }}
              >
                <Flex justify="space-between" align="flex-start">
                  <Flex
                    w={10}
                    h={10}
                    bg="orange.50"
                    _dark={{ bg: "orange.950/30" }}
                    rounded="xl"
                    align="center"
                    justify="center"
                  >
                    <Icon as={DocIcon} color="orange.500" boxSize={5} />
                  </Flex>
                  <Badge colorPalette={docTypeInfo.color} variant="subtle" rounded="md" px={2}>
                    {docTypeInfo.label}
                  </Badge>
                </Flex>

                 <VStack align="flex-start" gap={1}>
                  <Text fontWeight="bold" fontSize="md" noOfLines={1} title={doc.displayName || doc.fileName} _dark={{ color: "white" }}>
                    {doc.displayName || doc.fileName}
                  </Text>
                  <Text fontSize="xs" color="gray.400" noOfLines={1} title={doc.fileName}>
                    {doc.fileName}
                  </Text>
                  <HStack fontSize="11px" color="gray.500" gap={2}>
                    {doc.fileSize && <Text>{formatBytes(doc.fileSize)}</Text>}
                    {doc.fileSize && <Text>•</Text>}
                    <Text>{doc.createdAt ? new Date(doc.createdAt).toLocaleDateString() : ""}</Text>
                  </HStack>
                </VStack>

                <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />

                <Flex justify="space-between" align="center">
                  <Button
                    size="xs"
                    variant="ghost"
                    colorPalette="orange"
                    gap={1.5}
                    onClick={() => onDownloadDocument(doc.id, doc.fileName)}
                  >
                    <Icon as={Download} boxSize={3.5} />
                    Preview / Download
                  </Button>
                  <Button
                    size="xs"
                    variant="ghost"
                    colorPalette="red"
                    gap={1.5}
                    onClick={() => onDeleteDocument(doc.id)}
                  >
                    <Icon as={Trash2} boxSize={3.5} />
                    Delete
                  </Button>
                </Flex>
              </Box>
            );
          })}
        </SimpleGrid>
      ) : (
        <Center
          py={16}
          borderWidth="1.5px"
          borderStyle="dashed"
          borderColor="gray.300"
          rounded="2xl"
          _dark={{ borderColor: "whiteAlpha.100" }}
        >
          <VStack gap={3}>
            <Icon as={FileText} boxSize={16} color="gray.300" />
            <Text fontSize="lg" fontWeight="bold" color="gray.500">
             Missing workshop documents
            </Text>
            <Text fontSize="sm" color="gray.400" textAlign="center" maxW="sm" px={4}>
              Add your professional certification documents, insurance policies, or contracts here to make management easier and build customer trust.
            </Text>
          </VStack>
        </Center>
      )}

      {/* DIALOG: ADD DOCUMENT */}
      <DialogRoot open={isOpen} onOpenChange={(e) => !e.open && setIsOpen(false)}>
        <Portal>
          <DialogBackdrop />
          <DialogPositioner>
            <DialogContent _dark={{ bg: "rgb(25, 36, 54)", color: "white" }}>
              <form onSubmit={handleSubmit}>
                <DialogHeader>
                  <DialogTitle fontSize="xl" fontWeight="bold">Add a New Document</DialogTitle>
                </DialogHeader>
                <DialogBody display="flex" flexDirection="column" gap={4}>
                  <Field.Root required>
                    <Field.Label fontWeight="semibold">Display Name</Field.Label>
                    <Input
                      placeholder="np. Certyfikat Autoryzacji ASO"
                      value={docName}
                      onChange={(e) => setDocName(e.target.value)}
                      _dark={{ bg: "rgb(15, 23, 42)" }}
                    />
                  </Field.Root>

                  <Field.Root required>
                    <Field.Label fontWeight="semibold">Document type</Field.Label>
                    <NativeSelect.Root>
                      <NativeSelect.Field
                        value={docType}
                        onChange={(e) => setDocType(e.target.value)}
                        _dark={{ bg: "rgb(15, 23, 42)" }}
                      >
                        <option value="Logo">Workshop logo</option>
                        <option value="GalleryImage">Photo from the gallery (Gallery)</option>
                        <option value="Certificate">Training Certificate / Authorization</option>
                        <option value="Insurance">Business Liability Insurance</option>
                        <option value="Invoice">VAT Invoice / Settlements</option>
                        <option value="Other">Other document</option>
                      </NativeSelect.Field>
                    </NativeSelect.Root>
                  </Field.Root>

                  <Field.Root required>
                    <Field.Label fontWeight="semibold">Choos file (PDF, PNG, JPG)</Field.Label>
                    <Input
                      type="file"
                      accept=".pdf,.png,.jpg,.jpeg"
                      onChange={handleFileChange}
                      pt={1}
                      _dark={{ bg: "rgb(15, 23, 42)" }}
                    />
                  </Field.Root>
                </DialogBody>
                <DialogFooter gap={2}>
                  <DialogActionTrigger asChild>
                    <Button type="button" variant="ghost" rounded="lg">Cancel</Button>
                  </DialogActionTrigger>
                  <Button type="submit" colorPalette="orange" rounded="lg">
                    Add and save
                  </Button>
                </DialogFooter>
              </form>
              <DialogCloseTrigger />
            </DialogContent>
          </DialogPositioner>
        </Portal>
      </DialogRoot>
    </VStack>
  );
};

export default WorkshopDocumentsPanel;
