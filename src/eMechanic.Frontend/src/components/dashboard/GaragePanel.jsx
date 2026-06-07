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
  Center,
  Badge,
  InputGroup,
  Input,
  Separator,
  Skeleton,
  Image,
  Spinner,
} from "@chakra-ui/react";
import {
  Search,
  Car,
  Plus,
  Wrench,
  Calendar,
  FileText,
  History,
  Trash2,
  FileImage,
} from "lucide-react";
import { downloadDocument } from "../../api/vehicles";

// --- VEHICLE IMAGE PREVIEW COMPONENT ---
export const VehicleImage = ({ vehicleId, documentId }) => {
  const [imgUrl, setImgUrl] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let active = true;
    const loadImg = async () => {
      try {
        const blob = await downloadDocument(vehicleId, documentId);
        if (active) {
          const url = URL.createObjectURL(blob);
          setImgUrl(url);
        }
      } catch (err) {
        console.error("Failed to load vehicle image:", err);
      } finally {
        if (active) setLoading(false);
      }
    };
    loadImg();
    return () => {
      active = false;
      if (imgUrl) {
        URL.revokeObjectURL(imgUrl);
      }
    };
  }, [vehicleId, documentId]);

  if (loading) {
    return (
      <Center h="160px" bg="gray.50" _dark={{ bg: "rgb(15, 23, 42)" }} rounded="xl">
        <Spinner size="sm" color="brand.500" />
      </Center>
    );
  }

  if (!imgUrl) {
    return (
      <Center h="160px" bg="gray.50" _dark={{ bg: "rgb(15, 23, 42)" }} rounded="xl" border="1px dashed" borderColor="gray.200" _darkBorder={{ borderColor: "whiteAlpha.100" }}>
        <Icon as={FileImage} boxSize={8} color="gray.350" />
      </Center>
    );
  }

  return (
    <Box h="160px" w="full" overflow="hidden" rounded="xl">
      <Image
        src={imgUrl}
        alt="Zdjęcie pojazdu"
        w="full"
        h="full"
        objectFit="cover"
      />
    </Box>
  );
};

// --- VEHICLE CARD COMPONENT ---
export const VehicleCard = ({ vehicle, onEdit, onDelete, onTimeline, onDocuments }) => {
  const getFuelTypeString = (type) => {
    switch (type) {
      case 1: return "Benzyna";
      case 2: return "Diesel";
      case 3: return "LPG";
      case 4: return "Elektryczny";
      case 5: return "Hybryda";
      case 6: return "Wodór";
      default: return "Inny";
    }
  };

  const getBodyTypeString = (type) => {
    switch (type) {
      case 1: return "Sedan";
      case 2: return "Hatchback";
      case 3: return "Kombi";
      case 4: return "SUV";
      case 5: return "Coupe";
      case 6: return "Kabriolet";
      case 7: return "Minivan";
      case 8: return "Pickup";
      case 9: return "Van";
      default: return "Inny";
    }
  };

  const photoDoc = vehicle.documents?.find((d) => d.documentType === 2);

  return (
    <Box
      p={6}
      bg="white"
      _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
      rounded="2xl"
      borderWidth="1px"
      borderColor="gray.200"
      boxShadow="0 10px 25px -5px rgba(59, 130, 246, 0.05), 0 8px 10px -6px rgba(59, 130, 246, 0.05)"
      display="flex"
      flexDirection="column"
      gap={4}
      position="relative"
      transition="all 0.3s cubic-bezier(0.4, 0, 0.2, 1)"
      _hover={{
        transform: "translateY(-4px)",
        boxShadow: "0 20px 30px -10px rgba(59, 130, 246, 0.12), 0 10px 15px -3px rgba(59, 130, 246, 0.08)",
        borderColor: "brand.300",
      }}
    >
      {photoDoc && (
        <VehicleImage vehicleId={vehicle.id} documentId={photoDoc.documentId} />
      )}
      <Flex justify="space-between" align="flex-start">
        <VStack align="flex-start" gap={1}>
          <Heading size="md" fontWeight="bold" _dark={{ color: "white" }}>
            {vehicle.manufacturer} {vehicle.model}
          </Heading>
          <Text fontSize="xs" color="gray.400" fontWeight="medium">
            VIN: {vehicle.vin}
          </Text>
        </VStack>
        <Badge
          colorPalette="brand"
          variant="solid"
          rounded="lg"
          px={3}
          py={1}
          fontSize="xs"
          fontWeight="bold"
          shadow="sm"
        >
          {vehicle.licensePlate}
        </Badge>
      </Flex>

      <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />

      <SimpleGrid columns={3} gap={3} fontSize="sm">
        <VStack align="flex-start" gap={0}>
          <Text color="gray.400" fontSize="10px" textTransform="uppercase" fontWeight="bold">
            Rok
          </Text>
          <Text fontWeight="semibold" color="gray.700" _dark={{ color: "gray.200" }}>
            {vehicle.productionYear}
          </Text>
        </VStack>
        <VStack align="flex-start" gap={0}>
          <Text color="gray.400" fontSize="10px" textTransform="uppercase" fontWeight="bold">
            Przebieg
          </Text>
          <Text fontWeight="semibold" color="gray.700" _dark={{ color: "gray.200" }}>
            {vehicle.mileageValue.toLocaleString()} {vehicle.mileageUnit === 2 ? "mi" : "km"}
          </Text>
        </VStack>
        <VStack align="flex-start" gap={0}>
          <Text color="gray.400" fontSize="10px" textTransform="uppercase" fontWeight="bold">
            Typ
          </Text>
          <Text fontWeight="semibold" color="gray.700" _dark={{ color: "gray.200" }}>
            {vehicle.vehicleType === 2 ? "Motocykl" : "Osobowy"}
          </Text>
        </VStack>
        <VStack align="flex-start" gap={0}>
          <Text color="gray.400" fontSize="10px" textTransform="uppercase" fontWeight="bold">
            Paliwo
          </Text>
          <Text fontWeight="semibold" color="gray.700" _dark={{ color: "gray.200" }}>
            {getFuelTypeString(vehicle.fuelType)}
          </Text>
        </VStack>
        <VStack align="flex-start" gap={0}>
          <Text color="gray.400" fontSize="10px" textTransform="uppercase" fontWeight="bold">
            Nadwozie
          </Text>
          <Text fontWeight="semibold" color="gray.700" _dark={{ color: "gray.200" }}>
            {vehicle.vehicleType === 2 ? "Brak" : getBodyTypeString(vehicle.bodyType)}
          </Text>
        </VStack>
        <VStack align="flex-start" gap={0}>
          <Text color="gray.400" fontSize="10px" textTransform="uppercase" fontWeight="bold">
            Poj. / Moc
          </Text>
          <Text fontWeight="semibold" color="gray.700" _dark={{ color: "gray.200" }}>
            {vehicle.engineCapacity ? vehicle.engineCapacity.toFixed(1) + "l" : "—"} / {vehicle.horsePower || "—"} KM
          </Text>
        </VStack>
      </SimpleGrid>

      <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />

      <Flex justify="flex-end" gap={2} mt={1} wrap="wrap">
        <Button
          type="button"
          size="xs"
          colorPalette="brand"
          variant="outline"
          rounded="lg"
          onClick={() => onDocuments(vehicle)}
          _hover={{ bg: "brand.50", _dark: { bg: "brand.900/20" } }}
        >
          <Icon as={FileText} boxSize={3.5} />
          Dokumenty
        </Button>
        <Button
          type="button"
          size="xs"
          colorPalette="purple"
          variant="outline"
          rounded="lg"
          onClick={() => onTimeline(vehicle)}
          _hover={{ bg: "purple.50", _dark: { bg: "purple.900/20" } }}
        >
          <Icon as={History} boxSize={3.5} />
          Historia
        </Button>
        <Button
          type="button"
          size="xs"
          colorPalette="blue"
          variant="outline"
          rounded="lg"
          onClick={() => onEdit(vehicle)}
          _hover={{ bg: "blue.50", _dark: { bg: "blue.900/20" } }}
        >
          Edytuj
        </Button>
        <Button
          type="button"
          size="xs"
          colorPalette="red"
          variant="outline"
          rounded="lg"
          onClick={() => onDelete(vehicle.id)}
          _hover={{ bg: "red.50", _dark: { bg: "red.900/20" } }}
        >
          <Icon as={Trash2} boxSize={3.5} />
          Usuń
        </Button>
      </Flex>
    </Box>
  );
};

// --- SKELETON COMPONENT ---
export const CardSkeleton = () => (
  <Box
    p={6}
    bg="white"
    _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
    rounded="2xl"
    borderWidth="1px"
    borderColor="gray.200"
    shadow="md"
    display="flex"
    flexDirection="column"
    gap={4}
  >
    <Flex justify="space-between" align="flex-start">
      <VStack align="flex-start" gap={2} flex={1}>
        <Skeleton h="20px" w="60%" />
        <Skeleton h="14px" w="40%" />
      </VStack>
      <Skeleton h="24px" w="80px" rounded="md" />
    </Flex>
    <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />
    <SimpleGrid columns={2} gap={3}>
      {[...Array(4)].map((_, i) => (
        <VStack key={i} align="flex-start" gap={1}>
          <Skeleton h="12px" w="40%" />
          <Skeleton h="16px" w="70%" />
        </VStack>
      ))}
    </SimpleGrid>
    <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />
    <Flex justify="flex-end" mt={1}>
      <Skeleton h="32px" w="80px" rounded="lg" />
    </Flex>
  </Box>
);

// --- MAIN GARAGE PANEL ---
export const GaragePanel = ({
  vehicles,
  loadingVehicles,
  searchVehicle,
  setSearchVehicle,
  onAddVehicleClick,
  onEditVehicleClick,
  onDeleteVehicle,
  onTimelineClick,
  onDocumentsClick,
  repairRequests,
}) => {
  const filteredVehicles = vehicles.filter((v) => {
    const q = searchVehicle.toLowerCase();
    return (
      v.manufacturer.toLowerCase().includes(q) ||
      v.model.toLowerCase().includes(q) ||
      v.licensePlate.toLowerCase().includes(q) ||
      v.vin.toLowerCase().includes(q)
    );
  });

  const activeRequestsCount = repairRequests?.filter(
    (r) => r.status === 1 || r.status === 2
  ).length || 0;

  const completedRequestsCount = repairRequests?.filter(
    (r) => r.status === 3 || r.status === 4
  ).length || 0;

  return (
    <VStack align="stretch" gap={6}>
      <Flex justify="space-between" align="center">
        <Box>
          <Heading size="2xl" fontWeight="black" tracking="tight" _dark={{ color: "white" }}>
            Mój Garaż
          </Heading>
          <Text color="gray.500" _dark={{ color: "gray.400" }} fontSize="md" mt={1}>
            Zarządzaj pojazdami zgłoszonymi do eMechanic
          </Text>
        </Box>
        <Button
          onClick={onAddVehicleClick}
          colorPalette="brand"
          rounded="xl"
          px={6}
          fontWeight="bold"
          shadow="lg"
          _hover={{ transform: "translateY(-1px)", shadow: "xl" }}
        >
          <Icon as={Plus} />
          Dodaj Pojazd
        </Button>
      </Flex>

      {/* Stats Bar */}
      <SimpleGrid columns={{ base: 1, md: 3 }} gap={6}>
        <Box
          p={6}
          bg="white"
          _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
          borderWidth="1px"
          borderColor="gray.200"
          rounded="2xl"
          boxShadow="0 10px 25px -5px rgba(59, 130, 246, 0.05)"
          position="relative"
          overflow="hidden"
        >
          <Flex align="center" gap={4}>
            <Flex
              w={12}
              h={12}
              bg="brand.50"
              rounded="xl"
              _dark={{ bg: "brand.900/30" }}
              align="center"
              justify="center"
            >
              <Icon as={Car} color="brand.500" boxSize={6} />
            </Flex>
            <VStack align="flex-start" gap={0}>
              <Text fontSize="xs" color="gray.400" fontWeight="bold" textTransform="uppercase">
                Moje Samochody
              </Text>
              <Text fontSize="3xl" fontWeight="black" _dark={{ color: "white" }}>
                {vehicles.length}
              </Text>
            </VStack>
          </Flex>
        </Box>

        <Box
          p={6}
          bg="white"
          _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
          borderWidth="1px"
          borderColor="gray.200"
          rounded="2xl"
          boxShadow="0 10px 25px -5px rgba(59, 130, 246, 0.05)"
        >
          <Flex align="center" gap={4}>
            <Flex
              w={12}
              h={12}
              bg="brand.50"
              rounded="xl"
              _dark={{ bg: "brand.900/30" }}
              align="center"
              justify="center"
            >
              <Icon as={Wrench} color="brand.500" boxSize={6} />
            </Flex>
            <VStack align="flex-start" gap={0}>
              <Text fontSize="xs" color="gray.400" fontWeight="bold" textTransform="uppercase">
                Aktywne Zlecenia
              </Text>
              <Text fontSize="3xl" fontWeight="black" _dark={{ color: "white" }}>
                {activeRequestsCount}
              </Text>
            </VStack>
          </Flex>
        </Box>

        <Box
          p={6}
          bg="white"
          _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
          borderWidth="1px"
          borderColor="gray.200"
          rounded="2xl"
          boxShadow="0 10px 25px -5px rgba(59, 130, 246, 0.05)"
        >
          <Flex align="center" gap={4}>
            <Flex
              w={12}
              h={12}
              bg="green.50"
              rounded="xl"
              _dark={{ bg: "green.900/30" }}
              align="center"
              justify="center"
            >
              <Icon as={Calendar} color="green.500" boxSize={6} />
            </Flex>
            <VStack align="flex-start" gap={0}>
              <Text fontSize="xs" color="gray.400" fontWeight="bold" textTransform="uppercase">
                Historia Napraw
              </Text>
              <Text fontSize="3xl" fontWeight="black" _dark={{ color: "white" }}>
                {completedRequestsCount}
              </Text>
            </VStack>
          </Flex>
        </Box>
      </SimpleGrid>

      {/* Garage search & Grid list */}
      <Box
        p={6}
        bg="white"
        _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
        borderWidth="1px"
        borderColor="gray.200"
        rounded="2xl"
        boxShadow="0 15px 35px -5px rgba(59, 130, 246, 0.06)"
      >
        <Flex justify="space-between" align="center" mb={6} wrap="wrap" gap={4}>
          <Heading size="md" fontWeight="bold">
            Lista Samochodów
          </Heading>
          <InputGroup maxW="300px" startElement={<Icon as={Search} />}>
            <Input
              placeholder="Filtruj markę, model lub rejestrację..."
              value={searchVehicle}
              onChange={(e) => setSearchVehicle(e.target.value)}
              _focus={{
                borderColor: "brand.500",
                borderWidth: "2px",
                outline: "none",
              }}
              rounded="xl"
              _dark={{ bg: "rgb(15, 23, 42)" }}
            />
          </InputGroup>
        </Flex>

        {loadingVehicles ? (
          <SimpleGrid columns={{ base: 1, md: 2, lg: 3 }} gap={6}>
            {[...Array(3)].map((_, i) => (
              <CardSkeleton key={i} />
            ))}
          </SimpleGrid>
        ) : filteredVehicles.length > 0 ? (
          <SimpleGrid columns={{ base: 1, md: 2, lg: 3 }} gap={6}>
            {filteredVehicles.map((vehicle) => (
              <VehicleCard
                key={vehicle.id}
                vehicle={vehicle}
                onEdit={onEditVehicleClick}
                onDelete={onDeleteVehicle}
                onTimeline={onTimelineClick}
                onDocuments={onDocumentsClick}
              />
            ))}
          </SimpleGrid>
        ) : (
          <Center py={16} flexDirection="column" borderWidth="1.5px" borderStyle="dashed" borderColor="gray.300" rounded="2xl" _dark={{ borderColor: "whiteAlpha.100" }}>
            <Icon as={Car} boxSize={16} color="gray.300" mb={4} />
            <Text fontSize="lg" fontWeight="bold" color="gray.500">
              Brak pojazdów
            </Text>
            <Text
              fontSize="sm"
              color="gray.400"
              textAlign="center"
              maxW="sm"
              mt={1}
              px={4}
            >
              {searchVehicle
                ? "Brak wyników spełniających kryteria wyszukiwania."
                : "Zarejestruj swój pierwszy pojazd, aby móc zlecać naprawy warsztatom."}
            </Text>
            {!searchVehicle && (
              <Button
                onClick={onAddVehicleClick}
                colorPalette="brand"
                size="sm"
                rounded="lg"
                mt={6}
                fontWeight="semibold"
                shadow="md"
              >
                Dodaj Pojazd
              </Button>
            )}
          </Center>
        )}
      </Box>
    </VStack>
  );
};

export default GaragePanel;
