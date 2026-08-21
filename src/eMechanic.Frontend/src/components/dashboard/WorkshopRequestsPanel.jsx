import React, { useState, useEffect } from "react";
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
  Input,
  Separator,
  Field,
  Textarea,
  DialogRoot,
  DialogBackdrop,
  DialogContent,
  DialogHeader,
  DialogBody,
  DialogFooter,
  DialogTitle,
  DialogCloseTrigger,
  DialogActionTrigger,
  Portal,
  DialogPositioner,
  Center,
} from "@chakra-ui/react";
import {
  Car,
  Clock,
  Sparkles,
  Wrench,
  User,
  AlertCircle,
  XCircle,
  CheckCircle2,
} from "lucide-react";
import DashboardListLayout from "./DashboardListLayout";

export const WorkshopRequestsPanel = ({
  repairRequests,
  loading,
  onProvideEstimation,
  userPreferences = {},
  pageNumber = 1,
  totalPages = 1,
  pageSize = 5,
  onPageChange,
  onPageSizeChange,
  searchPhrase = "",
  onSearchChange,
  statusFilter = "All",
  onStatusFilterChange,
}) => {
  const [selectedRequest, setSelectedRequest] = useState(null);
  const [diagnosis, setDiagnosis] = useState("");
  const [cost, setCost] = useState("");
  const [isSubmitLoading, setIsSubmitLoading] = useState(false);

  const getRequestStatusNumber = (status) => {
    if (typeof status === "number") return status;
    if (typeof status === "string") {
      switch (status) {
        case "New":
        case "Created":
        case "Pending":
        case "Submitted":
        case "Opened":
        case "1":
          return 1;
        case "Estimated":
        case "Proposed":
        case "2":
          return 2;
        case "Accepted":
        case "Approved":
        case "3":
          return 3;
        case "Rejected":
        case "Declined":
        case "4":
          return 4;
        default:
          return 1;
      }
    }
    return 1;
  };

  const getStatusDetails = (status) => {
    const statusNum = getRequestStatusNumber(status);
    switch (statusNum) {
      case 1:
        return {
          label: "New",
          colorPalette: "blue",
          borderColor: "blue.200",
          icon: Clock,
          desc: "The order is awaiting your diagnosis and quote.",
        };
      case 2:
        return {
          label: "Quoted",
          colorPalette: "orange",
          borderColor: "orange.200",
          icon: Sparkles,
          desc: "The quote has been sent to the customer. Awaiting approval.",
        };
      case 3:
        return {
          label: "Approved",
          colorPalette: "green",
          borderColor: "green.200",
          icon: CheckCircle2,
          desc: "The customer accepted the quote. The order has become an active repair.",
        };
      case 4:
        return {
          label: "Rejected",
          colorPalette: "red",
          borderColor: "red.200",
          icon: XCircle,
          desc: "The customer rejected that quote.",
        };
      default:
        return { label: "Unknown", colorPalette: "gray", borderColor: "gray.200", icon: Clock, desc: "" };
    }
  };

  const filteredRequests = repairRequests;

  const handleOpenEstimation = (req) => {
    setSelectedRequest(req);
    setDiagnosis(req.diagnosis || "");
    setCost(req.estimatedCostAmount !== null ? req.estimatedCostAmount.toString() : "");
  };

  const handleEstimationSubmit = async (e) => {
    e.preventDefault();
    if (!diagnosis.trim() || !cost || parseFloat(cost) <= 0) {
      return;
    }
    setIsSubmitLoading(true);
    try {
      await onProvideEstimation(selectedRequest.id, diagnosis, parseFloat(cost));
      setSelectedRequest(null);
    } finally {
      setIsSubmitLoading(false);
    }
  };

  const filters = [
    {
      id: "status",
      label: "Status",
      value: statusFilter,
      onChange: onStatusFilterChange,
      options: [
        { value: "All", label: "Wszystkie", icon: Car, color: "orange.500" },
        { value: "Pending", label: "Nowe", icon: Clock, color: "blue.500" },
        { value: "Estimated", label: "Wycenione", icon: Sparkles, color: "orange.500" },
        { value: "Accepted", label: "Zaakceptowane", icon: CheckCircle2, color: "green.500" },
        { value: "Rejected", label: "Odrzucone", icon: XCircle, color: "red.500" },
      ]
    }
  ];

  return (
    <DashboardListLayout
      title="Client requests"
      subtitle="Manage service requests, perform diagnostics, and send cost estimates"
      filters={filters}
      totalItemsLabel={`Pokazano: ${filteredRequests.length} (na stronie)`}
      currentPage={pageNumber}
      totalPages={totalPages}
      pageSize={pageSize}
      onPageChange={onPageChange}
      onPageSizeChange={onPageSizeChange}
      searchPhrase={searchPhrase}
      onSearchChange={onSearchChange}
      loading={loading}
      empty={filteredRequests.length === 0}
      emptyState={
        <Center py={16} flexDirection="column" borderWidth="1.5px" borderStyle="dashed" borderColor="gray.300" rounded="2xl" _dark={{ borderColor: "whiteAlpha.100" }}>
          <Icon as={Clock} boxSize={16} color="gray.300" mb={4} />
          <Text fontSize="lg" fontWeight="bold" color="gray.500">
            {repairRequests.length > 0 ? "No orders with this status" : "No orders"}
          </Text>
          <Text fontSize="sm" color="gray.400" textAlign="center" mt={1} px={4}>
            {repairRequests.length > 0 ? "No orders matching the selected filter were found." : "You currently have no repair requests submitted by customers."}
          </Text>
        </Center>
      }
    >
      <VStack gap={5} align="stretch">
        {filteredRequests.map((req) => {
          const statusNum = getRequestStatusNumber(req.status);
          const statusInfo = getStatusDetails(statusNum);
          const canEstimate = statusNum === 1;
          const isRejected = statusNum === 4;
          const prefs = userPreferences[req.vehicle?.userId];

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
              boxShadow="0 10px 25px -5px rgba(249, 115, 22, 0.03), 0 8px 10px -6px rgba(249, 115, 22, 0.03)"
              display="flex"
              flexDirection="column"
              gap={4}
              transition="all 0.3s cubic-bezier(0.4, 0, 0.2, 1)"
              _hover={{
                boxShadow: "0 20px 30px -10px rgba(249, 115, 22, 0.08), 0 10px 15px -3px rgba(249, 115, 22, 0.04)",
                borderColor: "orange.200",
              }}
            >
              <Flex justify="space-between" align="center" wrap="wrap" gap={2}>
                <HStack gap={3}>
                  <Flex
                    w={10}
                    h={10}
                    bg="orange.50"
                    _dark={{ bg: "orange.950/30" }}
                    rounded="xl"
                    align="center"
                    justify="center"
                  >
                    <Icon as={Car} color="orange.500" boxSize={5} />
                  </Flex>
                  <VStack align="flex-start" gap={0}>
                    <Heading size="sm" fontWeight="bold" _dark={{ color: "white" }}>
                      {req.vehicle?.manufacturer && req.vehicle?.model 
                        ? `${req.vehicle.manufacturer} ${req.vehicle.model}` 
                        : "Vehicle"}
                    </Heading>
                    <Text fontSize="11px" color="gray.400" fontWeight="semibold">
                      License Plate: {req.vehicle?.licensePlate || "—"} | VIN: {req.vehicle?.vin || "—"}
                    </Text>
                  </VStack>
                </HStack>
                <Badge
                  colorPalette={statusInfo.colorPalette}
                  variant="subtle"
                  px={3}
                  py={1}
                  rounded="xl"
                  borderWidth="1px"
                  borderColor={statusInfo.borderColor}
                  display="flex"
                  alignItems="center"
                  gap={1.5}
                  shadow="sm"
                  fontWeight="bold"
                >
                  <Icon as={statusInfo.icon} boxSize={3.5} />
                  {statusInfo.label}
                </Badge>
              </Flex>

              <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />

              <VStack align="stretch" gap={3}>
                <Box>
                  <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                    Description
                  </Text>
                  <Text mt={1} color="gray.700" _dark={{ color: "gray.300" }} fontSize="sm">
                    {req.description}
                  </Text>
                </Box>

                {req.vehicle && (
                  <Box bg="gray.50" _dark={{ bg: "rgb(15, 23, 42)" }} p={3} rounded="xl">
                    <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase" mb={1.5}>
                      Client & Vehicle Data
                    </Text>
                    <SimpleGrid columns={{ base: 1, sm: 2 }} gap={2} fontSize="xs">
                      <HStack gap={1.5}>
                        <Icon as={User} color="orange.500" boxSize={3.5} />
                        <Text fontWeight="semibold" color="gray.600" _dark={{ color: "gray.300" }}>
                          Client: {req.clientName || "Brak danych"}
                        </Text>
                      </HStack>
                      <HStack gap={1.5}>
                        <Icon as={Sparkles} color="orange.500" boxSize={3.5} />
                        <Text color="gray.600" _dark={{ color: "gray.300" }}>
                          Mileage: {req.vehicle.mileageValue || "—"} {req.vehicle.mileageUnit === 2 ? "mi" : "km"}
                        </Text>
                      </HStack>
                    </SimpleGrid>
                  </Box>
                )}

                {/* Repair preferences */}
                {prefs && (
                  <Box bg="orange.50/20" _dark={{ bg: "orange.950/10" }} p={3.5} rounded="xl" borderWidth="1px" borderColor="orange.100/30" _darkBorder={{ borderColor: "orange.900/15" }}>
                    <Text fontSize="10px" fontWeight="bold" color="orange.600" _dark={{ color: "orange.400" }} textTransform="uppercase" mb={1.5}>
                      Customer Repair Preferences
                    </Text>
                    <SimpleGrid columns={{ base: 1, sm: 2 }} gap={2.5} fontSize="xs">
                      <HStack gap={1.5}>
                        <Text fontWeight="semibold" color="gray.500">Części:</Text>
                        <Badge colorPalette="orange" variant="subtle" size="sm" rounded="md">
                          {prefs.partsPreference === 1 || prefs.partsPreference === "Economy" ? "Ekonomiczne (zamienniki)" :
                           prefs.partsPreference === 2 || prefs.partsPreference === "Balanced" ? "Zbalansowane (OEM/Zamienniki)" :
                           prefs.partsPreference === 3 || prefs.partsPreference === "Premium" ? "Premium (tylko OEM)" : "Brak preferencji"}
                        </Badge>
                      </HStack>
                      <HStack gap={1.5}>
                        <Text fontWeight="semibold" color="gray.500">Czas:</Text>
                        <Badge colorPalette="orange" variant="subtle" size="sm" rounded="md">
                          {prefs.timelinePreference === 1 || prefs.timelinePreference === "Standard" ? "Standardowy" :
                           prefs.timelinePreference === 2 || prefs.timelinePreference === "Urgent" ? "Pilny (Ekspres)" : "Brak preferencji"}
                        </Badge>
                      </HStack>
                    </SimpleGrid>
                  </Box>
                )}

                {req.diagnosis && (
                  <Box>
                    <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                      Diagnoza warsztatu
                    </Text>
                    <Text mt={1} color="gray.700" _dark={{ color: "gray.300" }} fontSize="sm" fontWeight="medium">
                      {req.diagnosis}
                    </Text>
                  </Box>
                )}

                {req.estimatedCostAmount !== null && (
                  <HStack justify="space-between" bg="orange.50/50" _dark={{ bg: "orange.950/10" }} p={3} rounded="xl">
                    <Text fontSize="xs" fontWeight="bold" color="orange.700" _dark={{ color: "orange.300" }}>
                      Szacowany koszt naprawy
                    </Text>
                    <Text fontWeight="black" fontSize="lg" color="orange.600" _dark={{ color: "orange.400" }}>
                      {req.estimatedCostAmount} PLN
                    </Text>
                  </HStack>
                )}

                {isRejected && req.rejectionReason && (
                  <Box
                    p={3.5}
                    bg="red.50/50"
                    _dark={{ bg: "red.950/10" }}
                    rounded="xl"
                    borderWidth="1px"
                    borderColor="red.100"
                    _darkBorder={{ borderColor: "red.900/30" }}
                  >
                    <Text fontSize="10px" fontWeight="bold" color="red.500" textTransform="uppercase">
                      Reason for the customer's rejection
                    </Text>
                    <Text mt={1} color="red.700" _dark={{ color: "red.300" }} fontSize="sm">
                      {req.rejectionReason}
                    </Text>
                  </Box>
                )}

                {canEstimate && (
                  <VStack align="stretch" gap={3} mt={2}>
                    <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />
                    <Flex justify="flex-end">
                      <Button
                        size="sm"
                        colorPalette="orange"
                        variant="solid"
                        rounded="lg"
                        onClick={() => handleOpenEstimation(req)}
                        fontWeight="semibold"
                        gap={1.5}
                        shadow="md"
                      >
                        <Icon as={Wrench} boxSize={3.5} />
                        Get a quote / assessment
                      </Button>
                    </Flex>
                  </VStack>
                )}
              </VStack>
            </Box>
          );
        })}
      </VStack>

      {/* DIALOG: PROVIDE REPAIR ESTIMATION */}
      <DialogRoot open={!!selectedRequest} onOpenChange={(e) => !e.open && setSelectedRequest(null)}>
        <Portal>
          <DialogBackdrop />
          <DialogPositioner>
            <DialogContent _dark={{ bg: "rgb(25, 36, 54)", color: "white" }}>
              <form onSubmit={handleEstimationSubmit}>
                <DialogHeader>
                  <DialogTitle fontSize="xl" fontWeight="bold">Get a Quote</DialogTitle>
                  {selectedRequest && (
                    <Text fontSize="xs" color="gray.400" mt={1}>
                      Vehicle: {selectedRequest.vehicle?.manufacturer} {selectedRequest.vehicle?.model}
                    </Text>
                  )}
                </DialogHeader>
                <DialogBody display="flex" flexDirection="column" gap={4}>
                  <Field.Root required>
                    <Field.Label fontWeight="semibold">Fault Diagnosis</Field.Label>
                    <Textarea
                      placeholder="Describe the cause of the malfunction, which parts need to be replaced, and what work will be performed..."
                      value={diagnosis}
                      onChange={(e) => setDiagnosis(e.target.value)}
                      rows={4}
                      _dark={{ bg: "rgb(15, 23, 42)" }}
                    />
                  </Field.Root>

                  <Field.Root required>
                    <Field.Label fontWeight="semibold">Estimated gross cost (PLN)</Field.Label>
                    <Input
                      type="number"
                      placeholder="Wpisz kwotę w PLN, np. 500"
                      value={cost}
                      onChange={(e) => setCost(e.target.value)}
                      _dark={{ bg: "rgb(15, 23, 42)" }}
                    />
                  </Field.Root>
                </DialogBody>
                <DialogFooter gap={2}>
                  <DialogActionTrigger asChild>
                    <Button type="button" variant="ghost" rounded="lg">Cancel</Button>
                  </DialogActionTrigger>
                  <Button type="submit" loading={isSubmitLoading} colorPalette="orange" rounded="lg">
                    Send a quote
                  </Button>
                </DialogFooter>
              </form>
              <DialogCloseTrigger />
            </DialogContent>
          </DialogPositioner>
        </Portal>
      </DialogRoot>
    </DashboardListLayout>
  );
};

export default WorkshopRequestsPanel;
