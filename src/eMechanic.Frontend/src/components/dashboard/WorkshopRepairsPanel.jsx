import React, { useState } from "react";
import {
  Box,
  Flex,
  HStack,
  Button,
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
  Portal,
  DialogPositioner,
} from "@chakra-ui/react";
import { Car, Wrench, Check, Play, CheckCircle2, User, Calendar, CreditCard, Clock } from "lucide-react";
import DashboardListLayout from "./DashboardListLayout";

export const WorkshopRepairsPanel = ({
  repairs,
  loading,
  onStartRepair,
  onCompleteRepair,
  userPreferences = {},
  pageNumber = 1,
  totalPages = 1,
  pageSize = 6,
  onPageChange,
  onPageSizeChange,
  searchPhrase = "",
  onSearchChange,
  statusFilter = "All",
  onStatusFilterChange,
}) => {
  const [selectedRepair, setSelectedRepair] = useState(null);
  const [finalCost, setFinalCost] = useState("");
  const [isSubmitLoading, setIsSubmitLoading] = useState(false);

  const getStatusNumber = (status) => {
    if (typeof status === "number") return status;
    if (typeof status === "string") {
      switch (status) {
        case "Scheduled":
        case "0":
          return 0;
        case "InProgress":
        case "1":
          return 1;
        case "Completed":
        case "2":
          return 2;
        case "Paid":
        case "3":
          return 3;
        default:
          return 0;
      }
    }
    return 0;
  };

  const getRepairStatusDetails = (status) => {
    const statusNum = getStatusNumber(status);
    switch (statusNum) {
      case 0:
        return {
          label: "Scheduled",
          colorPalette: "blue",
          borderColor: "blue.200",
          icon: Calendar,
          desc: "The repair is scheduled. Work is set to begin.",
        };
      case 1:
        return {
          label: "In progress",
          colorPalette: "orange",
          borderColor: "orange.200",
          icon: Wrench,
          desc: "The vehicle is currently being repaired.",
        };
      case 2:
        return {
          label: "Completed",
          colorPalette: "yellow",
          borderColor: "yellow.200",
          icon: CheckCircle2,
          desc: "Work completed. Awaiting payment from the client.",
        };
      case 3:
        return {
          label: "Paid",
          colorPalette: "green",
          borderColor: "green.200",
          icon: CreditCard,
          desc: "Repair completed and paid for by the customer.",
        };
      default:
        return { label: "Unknown", colorPalette: "gray", borderColor: "gray.200", icon: Clock, desc: "" };
    }
  };

  const handleOpenCompleteModal = (rep) => {
    setSelectedRepair(rep);
    const estCost = rep.estimatedCost?.amount || rep.estimatedCostAmount || "";
    setFinalCost(estCost.toString());
  };

  const handleCompleteSubmit = async (e) => {
    e.preventDefault();
    if (!finalCost || parseFloat(finalCost) <= 0) {
      return;
    }
    setIsSubmitLoading(true);
    try {
      await onCompleteRepair(selectedRepair.id, parseFloat(finalCost));
      setSelectedRepair(null);
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
        { value: "All", label: "All", icon: Car, color: "orange.500" },
        { value: "Scheduled", label: "Scheduled", icon: Calendar, color: "blue.500" },
        { value: "InProgress", label: "In progress", icon: Wrench, color: "orange.500" },
        { value: "Completed", label: "Completed", icon: CheckCircle2, color: "yellow.500" },
        { value: "Paid", label: "Paid", icon: CreditCard, color: "green.500" },
      ]
    }
  ];

  return (
    <DashboardListLayout
      title="Repair Orders"
      subtitle="Track the progress of active repair jobs, create work orders, and approve costs once the repair is complete."
      filters={filters}
      totalItemsLabel={`Shown: ${repairs.length} (on page)`}
      currentPage={pageNumber}
      totalPages={totalPages}
      pageSize={pageSize}
      onPageChange={onPageChange}
      onPageSizeChange={onPageSizeChange}
      searchPhrase={searchPhrase}
      onSearchChange={onSearchChange}
      loading={loading}
      empty={repairs.length === 0}
      emptyState={
        <Center py={16} flexDirection="column" borderWidth="1.5px" borderStyle="dashed" borderColor="gray.300" rounded="2xl" _dark={{ borderColor: "whiteAlpha.100" }}>
          <Icon as={Wrench} boxSize={16} color="gray.300" mb={4} />
          <Text fontSize="lg" fontWeight="bold" color="gray.500">
            No repairs
          </Text>
          <Text fontSize="sm" color="gray.400" textAlign="center" mt={1} px={4}>
            You are not currently performing any repair work.
          </Text>
        </Center>
      }
    >
      <SimpleGrid columns={{ base: 1, lg: 2 }} gap={6}>
        {repairs.map((rep) => {
          const statusNum = getStatusNumber(rep.status);
          const statusInfo = getRepairStatusDetails(statusNum);
          const isScheduled = statusNum === 0;
          const isInProgress = statusNum === 1;
          const isCompleted = statusNum === 2;
          const isPaid = statusNum === 3;

          const estCost = rep.estimatedCost?.amount ?? rep.estimatedCostAmount;
          const finalCostVal = rep.finalCost?.amount ?? rep.finalCostAmount;
          const prefs = userPreferences[rep.vehicle?.userId];

          return (
            <Box
              key={rep.id}
              p={6}
              bg="white"
              _dark={{
                bg: "rgb(25, 36, 54)",
                borderColor: "whiteAlpha.100",
              }}
              borderWidth="1px"
              borderColor="gray.200"
              rounded="2xl"
              boxShadow="0 10px 25px -5px rgba(249, 115, 22, 0.03), 0 8px 10px -6px rgba(249, 115, 22, 0.03)"
              display="flex"
              flexDirection="column"
              justifyContent="space-between"
              gap={4}
              transition="all 0.3s cubic-bezier(0.4, 0, 0.2, 1)"
              _hover={{
                boxShadow: "0 20px 30px -10px rgba(249, 115, 22, 0.08), 0 10px 15px -3px rgba(249, 115, 22, 0.04)",
                borderColor: "orange.200",
              }}
            >
              {/* Header */}
              <Flex justify="space-between" align="center" wrap="wrap" gap={3}>
                <HStack gap={3}>
                  <Flex
                    w={10}
                    h={10}
                    bg="orange.50"
                    rounded="xl"
                    _dark={{ bg: "orange.950/30" }}
                    align="center"
                    justify="center"
                  >
                    <Icon as={Car} color="orange.500" boxSize={5} />
                  </Flex>
                  <VStack align="flex-start" gap={0}>
                    <Text fontWeight="bold" fontSize="md" _dark={{ color: "white" }}>
                      {rep.vehicle?.manufacturer} {rep.vehicle?.model}
                    </Text>
                    <Text fontSize="xs" color="gray.400" fontWeight="medium">
                      License plate: {rep.vehicle?.licensePlate || "—"} | VIN: {rep.vehicle?.vin || "—"}
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

              {/* Details */}
              <SimpleGrid columns={2} gap={4}>
                <Box>
                  <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                    Date created
                  </Text>
                  <Text mt={0.5} fontSize="sm" color="gray.700" _dark={{ color: "gray.200" }} fontWeight="medium">
                    {new Date(rep.createdAt).toLocaleDateString()}
                  </Text>
                </Box>
                <Box>
                  <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                    Malfunction / Diagnosis
                  </Text>
                  <Text mt={0.5} fontSize="sm" color="gray.700" _dark={{ color: "gray.200" }} noOfLines={1} title={rep.diagnosis || rep.description}>
                    {rep.diagnosis || rep.description || "Brak opisu"}
                  </Text>
                </Box>
              </SimpleGrid>

              {rep.vehicle && (
                <Box bg="gray.50" _dark={{ bg: "rgb(15, 23, 42)" }} p={3} rounded="xl">
                  <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase" mb={1.5}>
                    Customer Information
                  </Text>
                  <HStack gap={1.5} fontSize="xs">
                    <Icon as={User} color="orange.500" boxSize={3.5} />
                    <Text fontWeight="semibold" color="gray.600" _dark={{ color: "gray.300" }}>
                      Customer: {rep.clientName || "Brak danych"}
                    </Text>
                  </HStack>
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
                        {prefs.partsPreference === 1 || prefs.partsPreference === "Economy" ? "Economical (substitutes)" :
                         prefs.partsPreference === 2 || prefs.partsPreference === "Balanced" ? "Balanced (OEM/Replacement Parts)" :
                         prefs.partsPreference === 3 || prefs.partsPreference === "Premium" ? "Premium (OEM only)" : "No preference"}
                      </Badge>
                    </HStack>
                    <HStack gap={1.5}>
                      <Text fontWeight="semibold" color="gray.500">Czas:</Text>
                      <Badge colorPalette="orange" variant="subtle" size="sm" rounded="md">
                        {prefs.timelinePreference === 1 || prefs.timelinePreference === "Standard" ? "Standard" :
                         prefs.timelinePreference === 2 || prefs.timelinePreference === "Urgent" ? "Urgent (Express)" : "No preference"}
                      </Badge>
                    </HStack>
                  </SimpleGrid>
                </Box>
              )}

              <Box>
                <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                  Repair status
                </Text>
                <Text mt={0.5} fontSize="sm" color="gray.600" _dark={{ color: "gray.300" }}>
                  {statusInfo.desc}
                </Text>
              </Box>

              <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />

              {/* Costs & Action buttons */}
              <Flex justify="space-between" align="center" wrap="wrap" gap={3}>
                <Box>
                  <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                    {isCompleted || isPaid ? "Final cost" : "Estimated cost"}
                  </Text>
                  <Text fontSize="xl" fontWeight="black" color="orange.500">
                    {isCompleted || isPaid
                      ? `${finalCostVal?.toLocaleString() || "—"} PLN`
                      : `${estCost?.toLocaleString() || "—"} PLN`}
                  </Text>
                </Box>

                {isScheduled && (
                  <Button
                    size="sm"
                    colorPalette="orange"
                    variant="solid"
                    rounded="lg"
                    onClick={() => onStartRepair(rep.id)}
                    gap={1.5}
                    fontWeight="bold"
                    shadow="md"
                  >
                    <Icon as={Play} boxSize={3.5} />
                    Start the repair
                  </Button>
                )}

                {isInProgress && (
                  <Button
                    size="sm"
                    colorPalette="orange"
                    variant="outline"
                    borderColor="orange.500"
                    color="orange.500"
                    _hover={{ bg: "orange.50", _dark: { bg: "orange.900/20" } }}
                    rounded="lg"
                    onClick={() => handleOpenCompleteModal(rep)}
                    gap={1.5}
                    fontWeight="bold"
                  >
                    <Icon as={CheckCircle2} boxSize={3.5} />
                    Finish the repair
                  </Button>
                )}

                {isCompleted && (
                  <Badge colorPalette="yellow" variant="subtle" px={2} py={1} rounded="lg">
                    Waiting for payment
                  </Badge>
                )}

                {isPaid && (
                  <HStack color="green.500" fontSize="sm" fontWeight="bold" gap={1.5}>
                    <Icon as={Check} boxSize={4} />
                    <Text>Completed and Paid</Text>
                  </HStack>
                )}
              </Flex>
            </Box>
          );
        })}
      </SimpleGrid>

      {/* DIALOG: COMPLETE REPAIR */}
      <DialogRoot open={!!selectedRepair} onOpenChange={(e) => !e.open && setSelectedRepair(null)}>
        <Portal>
          <DialogBackdrop />
          <DialogPositioner>
            <DialogContent _dark={{ bg: "rgb(25, 36, 54)", color: "white" }}>
              <form onSubmit={handleCompleteSubmit}>
                <DialogHeader>
                  <DialogTitle fontSize="xl" fontWeight="bold">Zakończ Naprawę</DialogTitle>
                  {selectedRepair && (
                    <Text fontSize="xs" color="gray.400" mt={1}>
                      Vehicle: {selectedRepair.vehicle?.manufacturer} {selectedRepair.vehicle?.model}
                    </Text>
                  )}
                </DialogHeader>
                <DialogBody display="flex" flexDirection="column" gap={4}>
                  <Field.Root required>
                    <Field.Label fontWeight="semibold">Final gross cost (PLN)</Field.Label>
                    <Input
                      type="number"
                      placeholder="Enter the final cost, e.g., 550"
                      value={finalCost}
                      onChange={(e) => setFinalCost(e.target.value)}
                      _dark={{ bg: "rgb(15, 23, 42)" }}
                    />
                    <Text fontSize="xs" color="gray.400" mt={1}>
                     Enter the actual amount shown on the invoice or receipt for the customer.
                    </Text>
                  </Field.Root>
                </DialogBody>
                <DialogFooter gap={2}>
                  <DialogActionTrigger asChild>
                    <Button type="button" variant="ghost" rounded="lg">Cancel</Button>
                  </DialogActionTrigger>
                  <Button type="submit" loading={isSubmitLoading} colorPalette="orange" rounded="lg">
                    Confirm
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

export default WorkshopRepairsPanel;
