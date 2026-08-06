import React from "react";
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
  Center,
  Badge,
  Input,
  Separator,
  Skeleton,
  Field,
} from "@chakra-ui/react";
import {
  Car,
  Clock,
  Sparkles,
  FileText,
  Check,
  X,
} from "lucide-react";

// --- FORMAT API ERROR HELPER ---
const cleanSummary = (text) => {
  if (!text) return "";
  return text
    .replace(/^(summary\s+repairs\s*\(closure\)\s*:?\s*)/i, "")
    .trim();
};

// --- RENDER MARKDOWN HELPER ---
const renderMarkdown = (text) => {
  if (!text) return null;
  const lines = text.split("\n");
  return lines.map((line, idx) => {
    const trimmed = line.trim();
    if (!trimmed) {
      return <Box key={idx} h={2} />;
    }

    const isHeading = trimmed.startsWith("#");
    if (isHeading) {
      const level = (trimmed.match(/^#+/) || ["#"])[0].length;
      const cleanHeader = trimmed.replace(/^#+\s+/, "");
      return (
        <Heading
          key={idx}
          as={`h${Math.min(level + 2, 6)}`}
          size="xs"
          color="purple.800"
          _dark={{ color: "purple.300" }}
          mt={3}
          mb={1.5}
          fontWeight="bold"
        >
          {cleanHeader}
        </Heading>
      );
    }

    const isBullet = trimmed.startsWith("- ") || trimmed.startsWith("* ");
    const cleanLine = isBullet ? trimmed.replace(/^[-*]\s+/, "") : line;

    // Parse **bold** tags
    const parts = cleanLine.split(/\*\*([\s\S]*?)\*\*/g);
    const parsedText = parts.map((part, pIdx) => {
      if (pIdx % 2 === 1) {
        return (
          <Text
            key={pIdx}
            as="span"
            fontWeight="bold"
            color="purple.800"
            _dark={{ color: "purple.300" }}
          >
            {part}
          </Text>
        );
      }
      return part;
    });

    if (isBullet) {
      return (
        <HStack key={idx} align="flex-start" gap={2} my={1} pl={4}>
          <Box
            as="span"
            color="purple.500"
            fontSize="md"
            mt="-2px"
            userSelect="none"
          >
            •
          </Box>
          <Text
            fontSize="sm"
            color="gray.700"
            _dark={{ color: "gray.300" }}
            flex={1}
            lineHeight="relaxed"
          >
            {parsedText}
          </Text>
        </HStack>
      );
    }

    return (
      <Text
        key={idx}
        fontSize="sm"
        color="gray.700"
        _dark={{ color: "gray.300" }}
        my={1.5}
        lineHeight="relaxed"
      >
        {parsedText}
      </Text>
    );
  });
};

export const RequestsPanel = ({
  repairRequests,
  loadingRequests,
  allWorkshopsMap,
  handleAcceptEstimation,
  rejectingRequestId,
  setRejectingRequestId,
  rejectInputReason,
  setRejectInputReason,
  handleRejectEstimationSubmit,
  repairSummaries,
  loadingSummaries,
  fetchAiSummary,
}) => {
  const getStatusDetails = (status) => {
    switch (status) {
      case 1:
        return {
          label: "Awaiting",
          color: "blue",
          desc: "The order is awaiting a diagnosis and a quote from the repair shop.",
        };
      case 2:
        return {
          label: "Valued",
          color: "yellow",
          desc: "The workshop has made a diagnosis. We are waiting for your approval.",
        };
      case 3:
        return {
          label: "Accepted",
          color: "green",
          desc: "The estimate has been approved. Repair work is underway.",
        };
      case 4:
        return {
          label: "Rejected",
          color: "red",
          desc: "You rejected the quote for this order.",
        };
      default:
        return { label: "Unknown", color: "gray", desc: "" };
    }
  };

  return (
    <VStack align="stretch" gap={6}>
      <Box>
        <Heading size="2xl" fontWeight="black" tracking="tight" _dark={{ color: "white" }}>
          Repair orders
        </Heading>
        <Text color="gray.500" _dark={{ color: "gray.400" }} fontSize="md" mt={1}>
          All repair requests for your cars sent to repair shops
        </Text>
      </Box>

      {loadingRequests ? (
        <VStack gap={4} align="stretch">
          {[...Array(3)].map((_, i) => (
            <Box
              key={i}
              p={6}
              bg="white"
              _dark={{ bg: "rgb(25, 36, 54)" }}
              rounded="2xl"
              borderWidth="1px"
              borderColor="gray.200"
              shadow="md"
            >
              <Flex justify="space-between" align="center">
                <Skeleton h="20px" w="150px" />
                <Skeleton h="20px" w="100px" />
              </Flex>
              <Skeleton h="40px" w="full" mt={4} />
            </Box>
          ))}
        </VStack>
      ) : repairRequests.length > 0 ? (
        <VStack gap={5} align="stretch">
          {repairRequests.map((req) => {
            const statusInfo = getStatusDetails(req.status);
            const isRejected = req.status === 4;
            const isEstimated = req.status === 2;
            const workshopName =
              allWorkshopsMap[req.workshopId]?.displayName ||
              allWorkshopsMap[req.workshopId]?.name ||
              "Workshop";

            return (
              <Box
                key={req.id}
                p={6}
                bg="white"
                _dark={{
                  bg: "rgb(25, 36, 54)",
                  borderColor: "whiteAlpha.100",
                }}
                rounded="2xl"
                borderWidth="1px"
                borderColor="gray.200"
                boxShadow="0 10px 25px -5px rgba(59, 130, 246, 0.05), 0 8px 10px -6px rgba(59, 130, 246, 0.05)"
                display="flex"
                flexDirection="column"
                gap={4}
                transition="all 0.3s cubic-bezier(0.4, 0, 0.2, 1)"
                _hover={{
                  boxShadow: "0 20px 30px -10px rgba(59, 130, 246, 0.1), 0 10px 15px -3px rgba(59, 130, 246, 0.05)",
                  borderColor: "brand.200",
                }}
              >
                {/* Header info */}
                <Flex justify="space-between" align="center" wrap="wrap" gap={3}>
                  <HStack gap={3}>
                    <Flex
                      w={10}
                      h={10}
                      bg="brand.50"
                      rounded="xl"
                      _dark={{ bg: "brand.900/30" }}
                      align="center"
                      justify="center"
                    >
                      <Icon as={Car} color="brand.500" boxSize={5} />
                    </Flex>
                    <VStack align="flex-start" gap={0}>
                      <Text fontWeight="bold" fontSize="md" _dark={{ color: "white" }}>
                        {req.vehicle?.manufacturer} {req.vehicle?.model}
                      </Text>
                      <Text fontSize="xs" color="gray.400" fontWeight="medium">
                        Plates: {req.vehicle?.licensePlate} | VIN: {req.vehicle?.vin}
                      </Text>
                      <Text fontSize="xs" fontWeight="semibold" color="brand.600" _dark={{ color: "brand.400" }}>
                        Workshop: {workshopName}
                      </Text>
                    </VStack>
                  </HStack>

                  <VStack align="flex-end" gap={1}>
                    <Badge
                      colorPalette={statusInfo.color}
                      variant="solid"
                      px={3}
                      py={1}
                      rounded="full"
                      shadow="sm"
                    >
                      {statusInfo.label}
                    </Badge>
                    <Text fontSize="10px" color="gray.400" fontWeight="medium">
                      Added: {new Date(req.createdAt).toLocaleDateString()}
                    </Text>
                  </VStack>
                </Flex>

                <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />

                {/* Description */}
                <Box>
                  <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                    Report Description
                  </Text>
                  <Text mt={1} color="gray.700" _dark={{ color: "gray.200" }} fontSize="sm">
                    {req.description}
                  </Text>
                </Box>

                {/* Diagnosis & Estimations */}
                {(req.diagnosis || req.estimatedCostAmount !== null) && (
                  <Box
                    p={4}
                    bg="gray.50"
                    _dark={{ bg: "rgb(15, 23, 42)" }}
                    rounded="xl"
                    borderWidth="1px"
                    borderColor="gray.100"
                    _darkBorder={{ borderColor: "whiteAlpha.100" }}
                    shadow="inner"
                  >
                    <SimpleGrid columns={{ base: 1, md: 2 }} gap={4}>
                      {req.diagnosis && (
                        <Box>
                          <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                            Workshop Diagnosis
                          </Text>
                          <Text mt={1} color="gray.700" _dark={{ color: "gray.200" }} fontSize="sm">
                            {req.diagnosis}
                          </Text>
                        </Box>
                      )}
                      {req.estimatedCostAmount !== null && (
                        <Box>
                          <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                            Estimated cost
                          </Text>
                          <Text mt={1} fontSize="lg" fontWeight="black" color="brand.600" _dark={{ color: "brand.400" }}>
                            {req.estimatedCostAmount.toLocaleString()} {req.estimatedCostCurrency || "PLN"}
                          </Text>
                        </Box>
                      )}
                    </SimpleGrid>
                  </Box>
                )}

                {/* Rejection Details */}
                {isRejected && req.rejectionReason && (
                  <Box
                    p={4}
                    bg="red.50"
                    _dark={{ bg: "red.900/10" }}
                    rounded="xl"
                    borderWidth="1px"
                    borderColor="red.100"
                    _darkBorder={{ borderColor: "red.900/30" }}
                  >
                    <Text fontSize="10px" fontWeight="bold" color="red.500" textTransform="uppercase">
                      Reason for rejecting the valuation
                    </Text>
                    <Text mt={1} color="red.700" _dark={{ color: "red.300" }} fontSize="sm">
                      {req.rejectionReason}
                    </Text>
                  </Box>
                )}

                {/* Summary Details (Closure) - Unified in purple card layout */}
                {req.summaryReport && (
                  <Box
                    p={5}
                    bg="purple.50"
                    _dark={{ bg: "purple.950/20" }}
                    rounded="xl"
                    borderWidth="1px"
                    borderColor="purple.200"
                    _darkBorder={{ borderColor: "purple.900/30" }}
                    boxShadow="0 4px 15px -3px rgba(139, 92, 246, 0.1)"
                  >
                    <HStack gap={2} mb={2}>
                      <Icon as={FileText} color="purple.500" />
                      <Text
                        fontSize="10px"
                        fontWeight="bold"
                        color="purple.700"
                        _dark={{ color: "purple.300" }}
                        textTransform="uppercase"
                        letterSpacing="wider"
                      >
                        Repair Summary
                      </Text>
                    </HStack>
                    <Box mt={1} className="markdown-body">
                      {renderMarkdown(cleanSummary(req.summaryReport))}
                    </Box>
                  </Box>
                )}

                {/* AI Summary Section */}
                {repairSummaries[req.id] && (
                  <Box
                    p={5}
                    bg="purple.50"
                    _dark={{ bg: "purple.950/20" }}
                    rounded="xl"
                    borderWidth="1px"
                    borderColor="purple.200"
                    _darkBorder={{ borderColor: "purple.900/30" }}
                    boxShadow="0 4px 15px -3px rgba(139, 92, 246, 0.1)"
                    mt={2}
                  >
                    <HStack gap={2} mb={2}>
                      <Icon as={Sparkles} color="purple.500" />
                      <Text
                        fontSize="10px"
                        fontWeight="bold"
                        color="purple.700"
                        _dark={{ color: "purple.300" }}
                        textTransform="uppercase"
                        letterSpacing="wider"
                      >
                        AI Summary (Gemini)
                      </Text>
                    </HStack>
                    <Box mt={1} className="markdown-body">
                      {renderMarkdown(repairSummaries[req.id])}
                    </Box>
                  </Box>
                )}

                <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />

                <Flex justify="flex-end" align="center" gap={3} mt={1}>
                  <Button
                    size="sm"
                    colorPalette="purple"
                    variant="subtle"
                    rounded="lg"
                    loading={loadingSummaries[req.id]}
                    onClick={() => fetchAiSummary(req.id)}
                    gap={1.5}
                    fontWeight="semibold"
                  >
                    <Icon as={Sparkles} boxSize={3.5} />
                    {repairSummaries[req.id] ? "Regeneruj podsumowanie AI" : "Podsumowanie AI"}
                  </Button>

                  {isEstimated && (
                    <Flex gap={3}>
                      <Button
                        size="sm"
                        colorPalette="red"
                        variant="outline"
                        rounded="lg"
                        onClick={() => setRejectingRequestId(req.id)}
                        fontWeight="semibold"
                      >
                        Reject the quote
                      </Button>
                      <Button
                        size="sm"
                        colorPalette="green"
                        variant="solid"
                        rounded="lg"
                        onClick={() => handleAcceptEstimation(req.id)}
                        fontWeight="semibold"
                        shadow="sm"
                      >
                        Accept the quote
                      </Button>
                    </Flex>
                  )}
                </Flex>

                {/* Inline Reject Form */}
                {rejectingRequestId === req.id && (
                  <Box
                    p={4}
                    bg="gray.50"
                    _dark={{ bg: "rgb(15, 23, 42)" }}
                    rounded="xl"
                    mt={2}
                    borderWidth="1px"
                    borderColor="gray.250"
                    _darkBorder={{ borderColor: "whiteAlpha.100" }}
                    shadow="md"
                  >
                    <VStack align="stretch" gap={3}>
                      <Field.Root required>
                        <Field.Label fontSize="sm" fontWeight="bold">Reason for rejecting the valuation</Field.Label>
                        <Input
                          placeholder="Wpisz powód (np. zbyt wysoki koszt)"
                          value={rejectInputReason}
                          onChange={(e) => setRejectInputReason(e.target.value)}
                          _dark={{ bg: "rgb(25, 36, 54)" }}
                          rounded="xl"
                          size="sm"
                        />
                      </Field.Root>
                      <Flex justify="flex-end" gap={2}>
                        <Button
                          size="xs"
                          variant="outline"
                          colorPalette="gray"
                          rounded="md"
                          onClick={() => {
                            setRejectingRequestId(null);
                            setRejectInputReason("");
                          }}
                        >
                          Cancel
                        </Button>
                        <Button
                          size="xs"
                          colorPalette="red"
                          rounded="md"
                          fontWeight="semibold"
                          onClick={() => handleRejectEstimationSubmit(req.id)}
                        >
                          Confirm
                        </Button>
                      </Flex>
                    </VStack>
                  </Box>
                )}
              </Box>
            );
          })}
        </VStack>
      ) : (
        <Center py={16} flexDirection="column" borderWidth="1.5px" borderStyle="dashed" borderColor="gray.350" rounded="2xl" _dark={{ borderColor: "whiteAlpha.100" }}>
          <Icon as={Clock} boxSize={16} color="gray.300" mb={4} />
          <Text fontSize="lg" fontWeight="bold" color="gray.500">
            No orders
          </Text>
          <Text fontSize="sm" color="gray.400" textAlign="center" mt={1} px={4}>
            Orders will appear here after you submit the repair request form from the workshop list view.
          </Text>
        </Center>
      )}
    </VStack>
  );
};

export default RequestsPanel;
