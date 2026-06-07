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
  Skeleton,
  Portal,
  DialogPositioner,
} from "@chakra-ui/react";
import { Car, Wrench, Clock, Check, Play, CheckCircle2, User } from "lucide-react";

export const WorkshopRepairsPanel = ({
  repairs,
  loading,
  onStartRepair,
  onCompleteRepair,
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
      case 0: // Scheduled / Planned
        return {
          label: "Zaplanowana",
          color: "blue",
          desc: "Naprawa zaplanowana. Czeka na rozpoczęcie prac.",
        };
      case 1: // InProgress
        return {
          label: "W trakcie",
          color: "orange",
          desc: "Pojazd jest obecnie w trakcie naprawy.",
        };
      case 2: // Completed / Done
        return {
          label: "Ukończona",
          color: "yellow",
          desc: "Prace zakończone. Oczekiwanie na płatność klienta.",
        };
      case 3: // Paid
        return {
          label: "Opłacona",
          color: "green",
          desc: "Naprawa ukończona i opłacona przez klienta.",
        };
      default:
        return { label: "Nieznany", color: "gray", desc: "" };
    }
  };

  const handleOpenCompleteModal = (rep) => {
    setSelectedRepair(rep);
    // Suggest estimated cost as default final cost
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

  return (
    <VStack align="stretch" gap={6}>
      <Box>
        <Heading size="2xl" fontWeight="black" tracking="tight" _dark={{ color: "white" }}>
          Zlecenia Napraw
        </Heading>
        <Text color="gray.500" _dark={{ color: "gray.400" }} fontSize="md" mt={1}>
          Śledź postęp aktywnych prac naprawczych, rozpoczynaj zlecenia i zatwierdzaj koszty po zakończeniu naprawy.
        </Text>
      </Box>

      {loading ? (
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
      ) : repairs.length > 0 ? (
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
                        Tablice: {rep.vehicle?.licensePlate || "—"} | VIN: {rep.vehicle?.vin || "—"}
                      </Text>
                      <HStack gap={1} fontSize="xs" color="gray.500" mt={0.5}>
                        <Icon as={User} boxSize={3} />
                        <Text fontWeight="semibold">
                          Klient: {rep.clientName || rep.clientEmail || "Klient"}
                        </Text>
                      </HStack>
                    </VStack>
                  </HStack>

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
                </Flex>

                <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />

                {/* Details */}
                <SimpleGrid columns={2} gap={4}>
                  <Box>
                    <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                      Data utworzenia
                    </Text>
                    <Text mt={0.5} fontSize="sm" color="gray.700" _dark={{ color: "gray.200" }} fontWeight="medium">
                      {new Date(rep.createdAt).toLocaleDateString()}
                    </Text>
                  </Box>
                  <Box>
                    <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                      Usterka / Diagnoza
                    </Text>
                    <Text mt={0.5} fontSize="sm" color="gray.700" _dark={{ color: "gray.200" }} noOfLines={1} title={rep.diagnosis || rep.description}>
                      {rep.diagnosis || rep.description || "Brak opisu"}
                    </Text>
                  </Box>
                </SimpleGrid>

                <Box>
                  <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                    Status prac
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
                      {isCompleted || isPaid ? "Koszt ostateczny" : "Koszt szacowany"}
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
                      Rozpocznij naprawę
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
                      Zakończ naprawę
                    </Button>
                  )}

                  {isCompleted && (
                    <Badge colorPalette="yellow" variant="subtle" px={2} py={1} rounded="lg">
                      Oczekiwanie na zapłatę
                    </Badge>
                  )}

                  {isPaid && (
                    <HStack color="green.500" fontSize="sm" fontWeight="bold" gap={1.5}>
                      <Icon as={Check} boxSize={4} />
                      <Text>Zakończono i Opłacono</Text>
                    </HStack>
                  )}
                </Flex>
              </Box>
            );
          })}
        </SimpleGrid>
      ) : (
        <Center py={16} flexDirection="column" borderWidth="1.5px" borderStyle="dashed" borderColor="gray.300" rounded="2xl" _dark={{ borderColor: "whiteAlpha.100" }}>
          <Icon as={Wrench} boxSize={16} color="gray.300" mb={4} />
          <Text fontSize="lg" fontWeight="bold" color="gray.500">
            Brak napraw
          </Text>
          <Text fontSize="sm" color="gray.400" textAlign="center" mt={1} px={4}>
            Obecnie nie prowadzisz żadnych prac naprawczych.
          </Text>
        </Center>
      )}

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
                      Pojazd: {selectedRepair.vehicle?.manufacturer} {selectedRepair.vehicle?.model}
                    </Text>
                  )}
                </DialogHeader>
                <DialogBody display="flex" flexDirection="column" gap={4}>
                  <Field.Root required>
                    <Field.Label fontWeight="semibold">Ostateczny koszt brutto (PLN)</Field.Label>
                    <Input
                      type="number"
                      placeholder="Wpisz ostateczny koszt, np. 550"
                      value={finalCost}
                      onChange={(e) => setFinalCost(e.target.value)}
                      _dark={{ bg: "rgb(15, 23, 42)" }}
                    />
                    <Text fontSize="xs" color="gray.400" mt={1}>
                      Podaj rzeczywistą kwotę na fakturze / paragonie dla klienta.
                    </Text>
                  </Field.Root>
                </DialogBody>
                <DialogFooter gap={2}>
                  <DialogActionTrigger asChild>
                    <Button type="button" variant="ghost" rounded="lg">Anuluj</Button>
                  </DialogActionTrigger>
                  <Button type="submit" loading={isSubmitLoading} colorPalette="orange" rounded="lg">
                    Zatwierdź i Zakończ
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

export default WorkshopRepairsPanel;
