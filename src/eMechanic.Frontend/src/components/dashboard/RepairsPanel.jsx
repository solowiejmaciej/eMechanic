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
  Badge,
  Separator,
  Spinner,
  Center,
} from "@chakra-ui/react";
import { Car, Wrench, CreditCard, Check } from "lucide-react";

export const RepairsPanel = ({
  repairs,
  loading,
  isProcessingPayment,
  onPay,
  vehicles,
  allWorkshopsMap,
}) => {
  const getRepairStatusLabel = (status) => {
    switch (status) {
      case "Scheduled":
      case 0:
        return {
          label: "Zaplanowana",
          color: "blue",
          desc: "Naprawa została zaplanowana w warsztacie.",
        };
      case "InProgress":
      case 1:
        return {
          label: "W trakcie",
          color: "orange",
          desc: "Pojazd jest obecnie naprawiany.",
        };
      case "Completed":
      case 2:
        return {
          label: "Ukończona",
          color: "yellow",
          desc: "Prace zakończone. Oczekiwanie na płatność.",
        };
      case "Paid":
      case 3:
        return {
          label: "Opłacona",
          color: "green",
          desc: "Naprawa zakończona i w pełni opłacona.",
        };
      default:
        return { label: "Nieznana", color: "gray", desc: "" };
    }
  };

  const getStatusNumber = (status) => {
    if (typeof status === "number") return status;
    switch (status) {
      case "Scheduled": return 0;
      case "InProgress": return 1;
      case "Completed": return 2;
      case "Paid": return 3;
      default: return 0;
    }
  };

  if (loading) {
    return (
      <VStack gap={6} align="stretch">
        <Heading size="2xl" fontWeight="black" tracking="tight">Aktywne Naprawy</Heading>
        <Center py={20}>
          <Spinner size="xl" color="brand.500" />
        </Center>
      </VStack>
    );
  }

  return (
    <VStack gap={6} align="stretch">
      <Box>
        <Heading size="2xl" fontWeight="black" tracking="tight" _dark={{ color: "white" }}>
          Aktywne Naprawy i Płatności
        </Heading>
        <Text color="gray.500" _dark={{ color: "gray.400" }} fontSize="md" mt={1}>
          Śledź stan trwających napraw Twoich pojazdów oraz dokonuj bezpiecznych płatności online.
        </Text>
      </Box>

      {repairs.length === 0 ? (
        <Center
          py={16}
          bg="white"
          _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
          borderWidth="1px"
          borderColor="gray.200"
          rounded="2xl"
          shadow="md"
        >
          <VStack gap={3}>
            <Icon as={Wrench} boxSize={12} color="gray.300" />
            <Text fontSize="lg" fontWeight="bold" color="gray.500">
              Brak aktywnych napraw
            </Text>
            <Text fontSize="xs" color="gray.400" textAlign="center" maxW="xs" px={4}>
              Po zaakceptowaniu wyceny zlecenia przez Ciebie, warsztat rozpocznie naprawę, która pojawi się w tym miejscu.
            </Text>
          </VStack>
        </Center>
      ) : (
        <SimpleGrid columns={{ base: 1, lg: 2 }} gap={6}>
          {repairs.map((rep) => {
            const statusNum = getStatusNumber(rep.status);
            const statusInfo = getRepairStatusLabel(statusNum);
            const vehicle = vehicles.find((v) => v.id === rep.vehicleId);
            const workshop = allWorkshopsMap[rep.workshopId];
            const cost = statusNum >= 2 ? rep.finalCost : rep.estimatedCost;
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
                boxShadow="0 10px 25px -5px rgba(59, 130, 246, 0.05), 0 8px 10px -6px rgba(59, 130, 246, 0.05)"
                display="flex"
                flexDirection="column"
                justifyContent="space-between"
                gap={4}
                transition="all 0.3s cubic-bezier(0.4, 0, 0.2, 1)"
                _hover={{
                  boxShadow: "0 20px 30px -10px rgba(59, 130, 246, 0.1), 0 10px 15px -3px rgba(59, 130, 246, 0.05)",
                  borderColor: "brand.200",
                }}
              >
                <Flex justify="space-between" align="center" wrap="wrap" gap={2}>
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
                        {vehicle
                          ? `${vehicle.manufacturer} ${vehicle.model}`
                          : "Pojazd"}
                      </Text>
                      <Text fontSize="xs" color="gray.400" fontWeight="medium">
                        Tablice: {vehicle?.licensePlate || "—"} | VIN: {vehicle?.vin || "—"}
                      </Text>
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

                <SimpleGrid columns={2} gap={4}>
                  <Box>
                    <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                      Warsztat
                    </Text>
                    <Text
                      mt={0.5}
                      fontSize="sm"
                      fontWeight="semibold"
                      color="gray.700"
                      _dark={{ color: "gray.200" }}
                    >
                      {workshop?.displayName || workshop?.name || "Warsztat"}
                    </Text>
                  </Box>
                  <Box>
                    <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                      Data utworzenia
                    </Text>
                    <Text mt={0.5} fontSize="sm" color="gray.600" _dark={{ color: "gray.300" }}>
                      {new Date(rep.createdAt).toLocaleDateString()}
                    </Text>
                  </Box>
                </SimpleGrid>

                <Box>
                  <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                    Status i Diagnoza
                  </Text>
                  <Text mt={0.5} fontSize="sm" color="gray.600" _dark={{ color: "gray.300" }}>
                    {statusInfo.desc}
                  </Text>
                </Box>

                <Flex justify="space-between" align="center" mt={2} wrap="wrap" gap={3}>
                  <Box>
                    <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                      Koszt
                    </Text>
                    <Text fontSize="xl" fontWeight="black" color="brand.600" _dark={{ color: "brand.400" }}>
                      {cost
                        ? `${cost.amount.toLocaleString()} ${cost.currency}`
                        : "Brak wyceny"}
                    </Text>
                  </Box>

                  {statusNum === 2 && (
                    <Button
                      type="button"
                      colorPalette="brand"
                      size="sm"
                      rounded="lg"
                      loading={isProcessingPayment[rep.id]}
                      onClick={() => onPay(rep.id)}
                      gap={1.5}
                      fontWeight="bold"
                      shadow="md"
                    >
                      <Icon as={CreditCard} boxSize={3.5} />
                      Opłać naprawę
                    </Button>
                  )}
                  {statusNum === 3 && (
                    <HStack color="green.500" fontSize="sm" fontWeight="bold" gap={1.5}>
                      <Icon as={Check} boxSize={4} />
                      <Text>Opłacono</Text>
                    </HStack>
                  )}
                </Flex>
              </Box>
            );
          })}
        </SimpleGrid>
      )}
    </VStack>
  );
};

export default RepairsPanel;
