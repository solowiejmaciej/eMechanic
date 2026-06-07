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
  Center,
  Badge,
  Input,
  Separator,
  Skeleton,
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
} from "@chakra-ui/react";
import {
  Car,
  Clock,
  Sparkles,
  FileText,
  Check,
  X,
  Wrench,
  User,
} from "lucide-react";

export const WorkshopRequestsPanel = ({
  repairRequests,
  loading,
  onProvideEstimation,
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
          label: "Nowe",
          color: "blue",
          desc: "Zlecenie oczekuje na Twoją diagnozę i wycenę.",
        };
      case 2:
        return {
          label: "Wycenione",
          color: "orange",
          desc: "Wycena została wysłana do klienta. Oczekiwanie na akceptację.",
        };
      case 3:
        return {
          label: "Zaakceptowane",
          color: "green",
          desc: "Klient zaakceptował wycenę. Zlecenie stało się aktywną naprawą.",
        };
      case 4:
        return {
          label: "Odrzucone",
          color: "red",
          desc: "Klient odrzucił tę wycenę.",
        };
      default:
        return { label: "Nieznany", color: "gray", desc: "" };
    }
  };

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

  return (
    <VStack align="stretch" gap={6}>
      <Box>
        <Heading size="2xl" fontWeight="black" tracking="tight" _dark={{ color: "white" }}>
          Zlecenia od Klientów
        </Heading>
        <Text color="gray.500" _dark={{ color: "gray.400" }} fontSize="md" mt={1}>
          Zarządzaj zgłoszeniami serwisowymi, przeprowadzaj diagnozę i wysyłaj wyceny kosztów
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
      ) : repairRequests.length > 0 ? (
        <VStack gap={5} align="stretch">
          {repairRequests.map((req) => {
            const statusNum = getRequestStatusNumber(req.status);
            const statusInfo = getStatusDetails(statusNum);
            const canEstimate = statusNum === 1;
            const isRejected = statusNum === 4;

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
                {/* Header info */}
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
                        {req.vehicle?.manufacturer} {req.vehicle?.model}
                      </Text>
                      <Text fontSize="xs" color="gray.400" fontWeight="medium">
                        Tablice: {req.vehicle?.licensePlate || "—"} | VIN: {req.vehicle?.vin || "—"}
                      </Text>
                      <HStack gap={1} fontSize="xs" color="gray.500" mt={0.5}>
                        <Icon as={User} boxSize={3} />
                        <Text fontWeight="semibold">
                          Klient: {req.clientName || req.clientEmail || "Klient"}
                        </Text>
                      </HStack>
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
                      Zgłoszono: {new Date(req.createdAt).toLocaleDateString()}
                    </Text>
                  </VStack>
                </Flex>

                <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />

                {/* Description */}
                <Box>
                  <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                    Opis usterki od klienta
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
                            Twoja diagnoza
                          </Text>
                          <Text mt={1} color="gray.700" _dark={{ color: "gray.200" }} fontSize="sm">
                            {req.diagnosis}
                          </Text>
                        </Box>
                      )}
                      {req.estimatedCostAmount !== null && (
                        <Box>
                          <Text fontSize="10px" fontWeight="bold" color="gray.400" textTransform="uppercase">
                            Wyceniony koszt
                          </Text>
                          <Text mt={1} fontSize="lg" fontWeight="black" color="orange.500">
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
                      Powód odrzucenia przez klienta
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
                        Dokonaj wyceny / diagnozy
                      </Button>
                    </Flex>
                  </VStack>
                )}
              </Box>
            );
          })}
        </VStack>
      ) : (
        <Center py={16} flexDirection="column" borderWidth="1.5px" borderStyle="dashed" borderColor="gray.300" rounded="2xl" _dark={{ borderColor: "whiteAlpha.100" }}>
          <Icon as={Clock} boxSize={16} color="gray.300" mb={4} />
          <Text fontSize="lg" fontWeight="bold" color="gray.500">
            Brak zleceń
          </Text>
          <Text fontSize="sm" color="gray.400" textAlign="center" mt={1} px={4}>
            Obecnie nie posiadasz żadnych zgłoszonych zleceń napraw od klientów.
          </Text>
        </Center>
      )}

      {/* DIALOG: PROVIDE REPAIR ESTIMATION */}
      <DialogRoot open={!!selectedRequest} onOpenChange={(e) => !e.open && setSelectedRequest(null)}>
        <Portal>
          <DialogBackdrop />
          <DialogPositioner>
            <DialogContent _dark={{ bg: "rgb(25, 36, 54)", color: "white" }}>
              <form onSubmit={handleEstimationSubmit}>
                <DialogHeader>
                  <DialogTitle fontSize="xl" fontWeight="bold">Wyceń Zlecenie</DialogTitle>
                  {selectedRequest && (
                    <Text fontSize="xs" color="gray.400" mt={1}>
                      Pojazd: {selectedRequest.vehicle?.manufacturer} {selectedRequest.vehicle?.model}
                    </Text>
                  )}
                </DialogHeader>
                <DialogBody display="flex" flexDirection="column" gap={4}>
                  <Field.Root required>
                    <Field.Label fontWeight="semibold">Diagnoza usterki</Field.Label>
                    <Textarea
                      placeholder="Opisz co jest przyczyną usterki, jakie części należy wymienić i jakie prace zostaną przeprowadzone..."
                      value={diagnosis}
                      onChange={(e) => setDiagnosis(e.target.value)}
                      rows={4}
                      _dark={{ bg: "rgb(15, 23, 42)" }}
                    />
                  </Field.Root>

                  <Field.Root required>
                    <Field.Label fontWeight="semibold">Szacowany koszt brutto (PLN)</Field.Label>
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
                    <Button type="button" variant="ghost" rounded="lg">Anuluj</Button>
                  </DialogActionTrigger>
                  <Button type="submit" loading={isSubmitLoading} colorPalette="orange" rounded="lg">
                    Wyślij wycenę
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

export default WorkshopRequestsPanel;
