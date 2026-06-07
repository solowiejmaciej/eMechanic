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
  Separator,
} from "@chakra-ui/react";
import {
  Car,
  Wrench,
  Clock,
  Star,
  FileText,
  User,
  Plus,
  Play,
  FolderOpen,
  MessageSquare,
} from "lucide-react";
import { useAuth } from "../context/AuthContext";
import { toaster } from "@/components/ui/toaster";

// Import API functions
import { getWorkshopRequests, provideEstimation } from "../api/repairRequests";
import { getWorkshopRepairs, startRepair, completeRepair } from "../api/repairs";
import { getReviews, getReviewStats, uploadWorkshopDocument, deleteWorkshopDocument, getWorkshopDocuments } from "../api/workshops";
import { getById as getVehicleById } from "../api/vehicles";

// Import Panels
import WorkshopRequestsPanel from "@/components/dashboard/WorkshopRequestsPanel";
import WorkshopRepairsPanel from "@/components/dashboard/WorkshopRepairsPanel";
import WorkshopDocumentsPanel from "@/components/dashboard/WorkshopDocumentsPanel";
import WorkshopReviewsPanel from "@/components/dashboard/WorkshopReviewsPanel";
import WorkshopProfilePanel from "@/components/dashboard/WorkshopProfilePanel";

// --- MOCK DATABASE FALLBACKS FOR OFFLINE RUNNING ---
const initialRequests = [
  {
    id: "req-1",
    status: 1, // Nowe
    createdAt: "2026-06-05T10:00:00Z",
    description: "Stukanie w przednim zawieszeniu przy skręcaniu oraz na nierównościach.",
    diagnosis: "",
    estimatedCostAmount: null,
    estimatedCostCurrency: "PLN",
    clientName: "Jan Kowalski",
    clientEmail: "jan.kowalski@gmail.com",
    vehicle: { manufacturer: "Skoda", model: "Octavia", licensePlate: "KR 12345", vin: "TMBGG7NE8G210492" }
  },
  {
    id: "req-2",
    status: 2, // Wycenione
    createdAt: "2026-06-04T12:30:00Z",
    description: "Wyciek oleju spod silnika, zapaliła się czerwona kontrolka ciśnienia.",
    diagnosis: "Uszkodzona uszczelka pokrywy zaworów. Konieczna wymiana uszczelki oraz mycie silnika.",
    estimatedCostAmount: 1200,
    estimatedCostCurrency: "PLN",
    clientName: "Anna Nowak",
    clientEmail: "anna.nowak@onet.pl",
    vehicle: { manufacturer: "BMW", model: "Seria 3", licensePlate: "WI 99999", vin: "WBA8E1C5XGF20194" }
  },
  {
    id: "req-3",
    status: 3, // Zaakceptowane
    createdAt: "2026-06-03T15:45:00Z",
    description: "Klimatyzacja słabo chłodzi, słychać głośny szum z nawiewów.",
    diagnosis: "Nieszczelność chłodnicy klimatyzacji (skraplacza). Konieczna wymiana chłodnicy oraz napełnienie czynnika.",
    estimatedCostAmount: 850,
    estimatedCostCurrency: "PLN",
    clientName: "Piotr Wiśniewski",
    clientEmail: "piotr.wisniewski@wp.pl",
    vehicle: { manufacturer: "Ford", model: "Focus", licensePlate: "GD 7777A", vin: "WF0FXXWGCF8D1049" }
  }
];

const initialRepairs = [
  {
    id: "rep-1",
    status: 0, // Zaplanowana
    createdAt: "2026-06-06T08:00:00Z",
    description: "Wymiana tarcz i klocków hamulcowych przód",
    diagnosis: "Wymiana tarcz i klocków hamulcowych przód",
    estimatedCostAmount: 900,
    estimatedCost: { amount: 900, currency: "PLN" },
    clientName: "Krzysztof Zieliński",
    clientEmail: "krzysztof.z@gmail.com",
    vehicle: { manufacturer: "Volkswagen", model: "Golf", licensePlate: "WA 44455", vin: "WVWZZZ1KZD810294" }
  },
  {
    id: "rep-2",
    status: 1, // InProgress
    createdAt: "2026-06-05T09:15:00Z",
    description: "Wymiana rozrządu oraz pompy wody",
    diagnosis: "Wymiana rozrządu oraz pompy wody",
    estimatedCostAmount: 2400,
    estimatedCost: { amount: 2400, currency: "PLN" },
    clientName: "Małgorzata Wójcik",
    clientEmail: "m.wojcik@poczta.pl",
    vehicle: { manufacturer: "Audi", model: "A4", licensePlate: "PO 88888", vin: "WAUZZZ8K9CA20492" }
  },
  {
    id: "rep-3",
    status: 2, // Ukończona
    createdAt: "2026-06-04T11:00:00Z",
    description: "Regeneracja alternatora",
    diagnosis: "Regeneracja alternatora",
    estimatedCostAmount: 600,
    estimatedCost: { amount: 600, currency: "PLN" },
    finalCostAmount: 650,
    finalCost: { amount: 650, currency: "PLN" },
    clientName: "Robert Lewandowski",
    clientEmail: "rl9@lewy.com",
    vehicle: { manufacturer: "Opel", model: "Astra", licensePlate: "DW 33322", vin: "W0L0AHL358G29482" }
  },
  {
    id: "rep-4",
    status: 3, // Opłacona
    createdAt: "2026-06-02T14:00:00Z",
    description: "Serwis olejowo-filtrowy",
    diagnosis: "Serwis olejowo-filtrowy",
    estimatedCostAmount: 450,
    estimatedCost: { amount: 450, currency: "PLN" },
    finalCostAmount: 450,
    finalCost: { amount: 450, currency: "PLN" },
    clientName: "Paweł Szymański",
    clientEmail: "p.szymanski@gmail.com",
    vehicle: { manufacturer: "Toyota", model: "Corolla", licensePlate: "GD 11223", vin: "JTDKN32E0010492" }
  }
];

const initialDocs = [
  {
    id: "doc-1",
    displayName: "Polisa OC Działalności",
    originalFileName: "oc_warsztat_2026.pdf",
    contentType: "application/pdf",
    fileSize: 1024 * 1205, // 1.2MB
    createdAt: "2026-01-10T08:00:00Z",
    type: "Insurance",
  },
  {
    id: "doc-2",
    displayName: "Certyfikat Autoryzacji Bosch Service",
    originalFileName: "certyfikat_bosch.png",
    contentType: "image/png",
    fileSize: 1024 * 512, // 512KB
    createdAt: "2026-03-15T12:00:00Z",
    type: "Certificate",
  }
];

const initialReviews = [
  {
    id: "rev-1",
    rating: 5,
    comment: "Świetny kontakt, szybka diagnoza usterki zawieszenia i ekspresowa naprawa. Cena zgodna z wyceną. Polecam!",
    clientName: "Jan Kowalski",
    createdAt: "2026-06-06T15:20:00Z",
  },
  {
    id: "rev-2",
    rating: 5,
    comment: "Wymiana rozrządu w Audi wykonana profesjonalnie. Dostałem zdjęcia starego paska i części. Bardzo uczciwy warsztat.",
    clientName: "Małgorzata Wójcik",
    createdAt: "2026-06-05T18:40:00Z",
  },
  {
    id: "rev-3",
    rating: 4,
    comment: "Klimatyzacja w końcu działa poprawnie, choć usługa trwała kilka godzin dłużej niż planowano. Mimo to polecam za fachowość.",
    clientName: "Piotr Wiśniewski",
    createdAt: "2026-06-04T12:15:00Z",
  }
];

const initialStats = {
  averageRating: 4.8,
  totalReviews: 3,
  distribution: { 5: 2, 4: 1, 3: 0, 2: 0, 1: 0 }
};


// --- FORMAT API ERROR HELPER ---
const formatErrorMsg = (err, fallback) => {
  if (err.response?.data?.errors) {
    const errorDetails = Object.values(err.response.data.errors).flat().join(", ");
    return errorDetails || fallback;
  }
  return err.response?.data?.detail || err.response?.data?.title || err.message || fallback;
};

const WorkshopDashboard = ({ activeMenu, setActiveMenu }) => {
  const { user } = useAuth();
  
  // States holding data from API
  const [repairRequests, setRepairRequests] = useState([]);
  const [repairs, setRepairs] = useState([]);
  const [documents, setDocuments] = useState([]);
  const [reviews, setReviews] = useState([]);
  const [stats, setStats] = useState(initialStats);
  
  const [loading, setLoading] = useState(true);
  const [isOfflineMode, setIsOfflineMode] = useState(false);

  // Profile Form States
  const [profileDisplayName, setProfileDisplayName] = useState("");
  const [profileEmail, setProfileEmail] = useState("");
  const [profilePhone, setProfilePhone] = useState("");
  const [profileCity, setProfileCity] = useState("");
  const [profileAddress, setProfileAddress] = useState("");
  const [profileDescription, setProfileDescription] = useState("");
  const [profileSubmitting, setProfileSubmitting] = useState(false);

  // Reusable API Fetch Helpers
  const fetchRequests = async () => {
    try {
      const reqData = await getWorkshopRequests({ PageNumber: 1, PageSize: 50 });
      const items = reqData.items || [];
      
      const itemsWithVehicles = await Promise.all(
        items.map(async (item) => {
          try {
            if (item.vehicleId) {
              const vehicle = await getVehicleById(item.vehicleId);
              return {
                ...item,
                vehicle,
                clientName: vehicle.clientFirstName && vehicle.clientLastName 
                  ? `${vehicle.clientFirstName} ${vehicle.clientLastName}` 
                  : (vehicle.clientEmail || "Brak danych klienta"),
                clientEmail: vehicle.clientEmail || "Brak e-maila"
              };
            }
          } catch (vehicleErr) {
            console.error(`Failed to fetch vehicle for ID ${item.vehicleId}:`, vehicleErr);
          }
          return {
            ...item,
            vehicle: null,
            clientName: "Brak danych klienta",
            clientEmail: "Brak e-maila"
          };
        })
      );

      setRepairRequests(itemsWithVehicles);
      return true;
    } catch (err) {
      console.error("Failed to fetch requests from API:", err);
      toaster.create({
        title: "Błąd pobierania",
        description: formatErrorMsg(err, "Nie udało się pobrać zleceń z API."),
        type: "error"
      });
      return false;
    }
  };

  const fetchRepairs = async () => {
    try {
      const repData = await getWorkshopRepairs({ PageNumber: 1, PageSize: 50 });
      const items = repData.items || [];
      
      const itemsWithVehicles = await Promise.all(
        items.map(async (item) => {
          try {
            if (item.vehicleId) {
              const vehicle = await getVehicleById(item.vehicleId);
              return {
                ...item,
                vehicle,
                clientName: vehicle.clientFirstName && vehicle.clientLastName 
                  ? `${vehicle.clientFirstName} ${vehicle.clientLastName}` 
                  : (vehicle.clientEmail || "Brak danych klienta"),
                clientEmail: vehicle.clientEmail || "Brak e-maila"
              };
            }
          } catch (vehicleErr) {
            console.error(`Failed to fetch vehicle for ID ${item.vehicleId}:`, vehicleErr);
          }
          return {
            ...item,
            vehicle: null,
            clientName: "Brak danych klienta",
            clientEmail: "Brak e-maila"
          };
        })
      );

      setRepairs(itemsWithVehicles);
      return true;
    } catch (err) {
      console.error("Failed to fetch repairs from API:", err);
      toaster.create({
        title: "Błąd pobierania",
        description: formatErrorMsg(err, "Nie udało się pobrać napraw z API."),
        type: "error"
      });
      return false;
    }
  };

  const fetchDocs = async () => {
    if (!user?.id) return false;
    try {
      const docsData = await getWorkshopDocuments(user.id, { PageNumber: 1, PageSize: 50 });
      setDocuments(docsData.items || []);
      return true;
    } catch (err) {
      console.error("Failed to fetch documents from API:", err);
      toaster.create({
        title: "Błąd pobierania",
        description: formatErrorMsg(err, "Nie udało się pobrać dokumentów z API."),
        type: "error"
      });
      return false;
    }
  };

  const fetchReviewsData = async () => {
    if (!user?.id) return false;
    try {
      const revs = await getReviews(user.id, { PageNumber: 1, PageSize: 50 });
      setReviews(revs.items || []);
      const rStats = await getReviewStats(user.id);
      setStats(rStats || initialStats);
      return true;
    } catch (err) {
      console.error("Failed to fetch reviews from API:", err);
      toaster.create({
        title: "Błąd pobierania",
        description: formatErrorMsg(err, "Nie udało się pobrać opinii z API."),
        type: "error"
      });
      return false;
    }
  };

  // Fetch all initial data
  useEffect(() => {
    const loadDashboardData = async () => {
      setLoading(true);
      
      const reqsOk = await fetchRequests();
      const repsOk = await fetchRepairs();
      
      let revsOk = true;
      let docsOk = true;
      if (user?.id) {
        revsOk = await fetchReviewsData();
        docsOk = await fetchDocs();
      }

      // Initialize Profile Fields
      setProfileDisplayName(user?.displayName || "");
      setProfileEmail(user?.email || "");
      setProfilePhone(user?.phone || "");
      setProfileCity(user?.city || "");
      setProfileAddress(user?.address || "");
      setProfileDescription(user?.description || "");

      setIsOfflineMode(!reqsOk || !repsOk || !revsOk || !docsOk);
      setLoading(false);
    };

    loadDashboardData();
  }, [user]);

  // Action: Submit diagnosis & cost estimation
  const handleProvideEstimation = async (requestId, diagnosis, cost) => {
    try {
      await provideEstimation(requestId, diagnosis, cost);
      await fetchRequests();
      
      toaster.create({
        title: "Wycena wysłana",
        description: "Wycena została pomyślnie wysłana i zapisana w API.",
        type: "success",
      });
    } catch (err) {
      console.error("Failed to submit estimation to API:", err);
      toaster.create({
        title: "Błąd wyceny",
        description: formatErrorMsg(err, "Nie udało się zapisać wyceny w API."),
        type: "error",
      });
    }
  };

  // Action: Start Repair
  const handleStartRepair = async (repairId) => {
    try {
      await startRepair(repairId);
      await fetchRepairs();

      toaster.create({
        title: "Naprawa rozpoczęta",
        description: "Status naprawy został zaktualizowany na serwerze.",
        type: "success",
      });
    } catch (err) {
      console.error("Failed to start repair on API:", err);
      toaster.create({
        title: "Błąd rozpoczęcia naprawy",
        description: formatErrorMsg(err, "Nie udało się zmienić statusu naprawy w API."),
        type: "error",
      });
    }
  };

  // Action: Complete Repair
  const handleCompleteRepair = async (repairId, finalCost) => {
    try {
      await completeRepair(repairId, finalCost);
      await fetchRepairs();

      toaster.create({
        title: "Naprawa zakończona",
        description: "Ostateczny koszt został zapisany na serwerze.",
        type: "success",
      });
    } catch (err) {
      console.error("Failed to complete repair on API:", err);
      toaster.create({
        title: "Błąd zakończenia naprawy",
        description: formatErrorMsg(err, "Nie udało się zapisać ostatecznego kosztu w API."),
        type: "error",
      });
    }
  };

  // Action: Upload Document
  const handleUploadDocument = async (mockDoc, actualFile) => {
    if (user?.id && actualFile) {
      try {
        const formData = new FormData();
        formData.append("file", actualFile);
        formData.append("displayName", mockDoc.displayName);
        formData.append("documentType", mockDoc.type);
        
        await uploadWorkshopDocument(formData);
        await fetchDocs();
        
        toaster.create({
          title: "Dodano dokument",
          description: `Dokument "${mockDoc.displayName}" został pomyślnie przesłany na serwer.`,
          type: "success",
        });
      } catch (err) {
        console.error("Failed to upload document to API:", err);
        toaster.create({
          title: "Błąd przesyłania dokumentu",
          description: formatErrorMsg(err, "Nie udało się zapisać dokumentu w API."),
          type: "error",
        });
      }
    } else {
      toaster.create({
        title: "Błąd",
        description: "Musisz wybrać fizyczny plik do przesłania.",
        type: "error",
      });
    }
  };

  // Action: Delete Document
  const handleDeleteDocument = async (docId) => {
    if (user?.id) {
      try {
        await deleteWorkshopDocument(docId);
        await fetchDocs();
        
        toaster.create({
          title: "Usunięto dokument",
          description: "Dokument został usunięty z serwera.",
          type: "success",
        });
      } catch (err) {
        console.error("Failed to delete document from API:", err);
        toaster.create({
          title: "Błąd usuwania dokumentu",
          description: formatErrorMsg(err, "Nie udało się usunąć dokumentu z API."),
          type: "error",
        });
      }
    }
  };

  // Action: Save profile details
  const handleProfileSubmit = (e) => {
    e.preventDefault();
    setProfileSubmitting(true);
    setTimeout(() => {
      setProfileSubmitting(false);
      toaster.create({
        title: "Zapisano dane profilowe",
        description: "Informacje o Twoim warsztacie zostały zaktualizowane pomyślnie.",
        type: "success",
      });
    }, 600);
  };

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

  // --- STATS COMPUTATION FOR THE 3 METRIC CARDS ---
  const activeRepairsCount = repairs.filter((r) => getStatusNumber(r.status) === 1).length;
  const vehiclesInShopCount = repairs.filter((r) => {
    const s = getStatusNumber(r.status);
    return s === 0 || s === 1 || s === 2;
  }).length;
  const newRequestsCount = repairRequests.filter((r) => getRequestStatusNumber(r.status) === 1).length;

  return (
    <Box minH="100vh" w="full" display="flex" bg="gray.100" _dark={{ bg: "#0F172A" }} pt="80px">
      
      {/* --- LEFT SIDEBAR --- */}
      <Box
        w="280px"
        bg="white"
        _dark={{ bg: "rgb(25, 36, 54)" }}
        borderRight="1px"
        borderColor="gray.200"
        _darkBorder={{ borderColor: "whiteAlpha.100" }}
        display="flex"
        flexDirection="column"
        justifyContent="space-between"
        p={6}
        position="sticky"
        top="80px"
        h="calc(100vh - 80px)"
      >
        <VStack align="stretch" gap={8}>
          {/* Dashboard Logo or Header */}
          <Box>
            <Text fontSize="10px" fontWeight="bold" color="orange.500" tracking="widest" textTransform="uppercase">
              Panel Mechanika
            </Text>
            <Heading size="md" fontWeight="bold" mt={0.5} _dark={{ color: "white" }}>
              {profileDisplayName || "Mój Warsztat"}
            </Heading>
            {isOfflineMode && (
              <Badge colorPalette="orange" variant="outline" mt={1}>
                Tryb offline
              </Badge>
            )}
          </Box>

          {/* Menu options */}
          <VStack align="stretch" gap={2}>
            <Button
              onClick={() => setActiveMenu("requests")}
              justifyContent="flex-start"
              variant="ghost"
              h="12"
              rounded="xl"
              color={activeMenu === "requests" ? "orange.600" : "gray.500"}
              bg={activeMenu === "requests" ? "orange.50" : "transparent"}
              _dark={{
                color: activeMenu === "requests" ? "orange.400" : "gray.400",
                bg: activeMenu === "requests" ? "whiteAlpha.100" : "transparent",
              }}
              gap={3}
            >
              <Icon as={Clock} boxSize={5} />
              Zlecenia
              {newRequestsCount > 0 && (
                <Badge colorPalette="orange" variant="solid" rounded="full" ml="auto" px={2}>
                  {newRequestsCount}
                </Badge>
              )}
            </Button>

            <Button
              onClick={() => setActiveMenu("repairs")}
              justifyContent="flex-start"
              variant="ghost"
              h="12"
              rounded="xl"
              color={activeMenu === "repairs" ? "orange.600" : "gray.500"}
              bg={activeMenu === "repairs" ? "orange.50" : "transparent"}
              _dark={{
                color: activeMenu === "repairs" ? "orange.400" : "gray.400",
                bg: activeMenu === "repairs" ? "whiteAlpha.100" : "transparent",
              }}
              gap={3}
            >
              <Icon as={Wrench} boxSize={5} />
              Aktywne Naprawy
              {activeRepairsCount > 0 && (
                <Badge colorPalette="blue" variant="solid" rounded="full" ml="auto" px={2}>
                  {activeRepairsCount}
                </Badge>
              )}
            </Button>

            <Button
              onClick={() => setActiveMenu("documents")}
              justifyContent="flex-start"
              variant="ghost"
              h="12"
              rounded="xl"
              color={activeMenu === "documents" ? "orange.600" : "gray.500"}
              bg={activeMenu === "documents" ? "orange.50" : "transparent"}
              _dark={{
                color: activeMenu === "documents" ? "orange.400" : "gray.400",
                bg: activeMenu === "documents" ? "whiteAlpha.100" : "transparent",
              }}
              gap={3}
            >
              <Icon as={FolderOpen} boxSize={5} />
              Dokumenty
            </Button>

            <Button
              onClick={() => setActiveMenu("reviews")}
              justifyContent="flex-start"
              variant="ghost"
              h="12"
              rounded="xl"
              color={activeMenu === "reviews" ? "orange.600" : "gray.500"}
              bg={activeMenu === "reviews" ? "orange.50" : "transparent"}
              _dark={{
                color: activeMenu === "reviews" ? "orange.400" : "gray.400",
                bg: activeMenu === "reviews" ? "whiteAlpha.100" : "transparent",
              }}
              gap={3}
            >
              <Icon as={Star} boxSize={5} />
              Opinie
            </Button>

            <Button
              onClick={() => setActiveMenu("profile")}
              justifyContent="flex-start"
              variant="ghost"
              h="12"
              rounded="xl"
              color={activeMenu === "profile" ? "orange.600" : "gray.500"}
              bg={activeMenu === "profile" ? "orange.50" : "transparent"}
              _dark={{
                color: activeMenu === "profile" ? "orange.400" : "gray.400",
                bg: activeMenu === "profile" ? "whiteAlpha.100" : "transparent",
              }}
              gap={3}
            >
              <Icon as={User} boxSize={5} />
              Profil
            </Button>
          </VStack>
        </VStack>
      </Box>

      {/* --- RIGHT CONTENT AREA --- */}
      <Box flex="1" p={8} overflowY="auto" h="calc(100vh - 80px)" display="flex" flexDirection="column" gap={8}>
        
        {/* Top metrics summary grid (always visible at top of dashboard workspace) */}
        <SimpleGrid columns={{ base: 1, md: 3 }} gap={6}>
          <Box
            p={5}
            bg="white"
            _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
            borderWidth="1px"
            borderColor="gray.200"
            rounded="2xl"
            boxShadow="md"
            transition="all 0.2s ease"
            _hover={{ transform: "translateY(-2px)" }}
          >
            <Flex align="center" gap={4}>
              <Flex
                w={12}
                h={12}
                bg="orange.50"
                _dark={{ bg: "orange.950/30" }}
                rounded="xl"
                align="center"
                justify="center"
              >
                <Icon as={Wrench} color="orange.500" boxSize={6} />
              </Flex>
              <VStack align="flex-start" gap={0}>
                <Text fontSize="xs" fontWeight="bold" color="gray.400" textTransform="uppercase">
                  W trakcie naprawy
                </Text>
                <Text fontSize="3xl" fontWeight="black" color="gray.800" _dark={{ color: "white" }}>
                  {activeRepairsCount}
                </Text>
              </VStack>
            </Flex>
          </Box>

          <Box
            p={5}
            bg="white"
            _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
            borderWidth="1px"
            borderColor="gray.200"
            rounded="2xl"
            boxShadow="md"
            transition="all 0.2s ease"
            _hover={{ transform: "translateY(-2px)" }}
          >
            <Flex align="center" gap={4}>
              <Flex
                w={12}
                h={12}
                bg="blue.50"
                _dark={{ bg: "blue.950/30" }}
                rounded="xl"
                align="center"
                justify="center"
              >
                <Icon as={Car} color="blue.500" boxSize={6} />
              </Flex>
              <VStack align="flex-start" gap={0}>
                <Text fontSize="xs" fontWeight="bold" color="gray.400" textTransform="uppercase">
                  Pojazdy w serwisie
                </Text>
                <Text fontSize="3xl" fontWeight="black" color="gray.800" _dark={{ color: "white" }}>
                  {vehiclesInShopCount}
                </Text>
              </VStack>
            </Flex>
          </Box>

          <Box
            p={5}
            bg="white"
            _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
            borderWidth="1px"
            borderColor="gray.200"
            rounded="2xl"
            boxShadow="md"
            transition="all 0.2s ease"
            _hover={{ transform: "translateY(-2px)" }}
          >
            <Flex align="center" gap={4}>
              <Flex
                w={12}
                h={12}
                bg="green.50"
                _dark={{ bg: "green.950/30" }}
                rounded="xl"
                align="center"
                justify="center"
              >
                <Icon as={Clock} color="green.500" boxSize={6} />
              </Flex>
              <VStack align="flex-start" gap={0}>
                <Text fontSize="xs" fontWeight="bold" color="gray.400" textTransform="uppercase">
                  Nowe zlecenia
                </Text>
                <Text fontSize="3xl" fontWeight="black" color="gray.800" _dark={{ color: "white" }}>
                  {newRequestsCount}
                </Text>
              </VStack>
            </Flex>
          </Box>
        </SimpleGrid>

        <Separator borderColor="gray.200" _dark={{ borderColor: "whiteAlpha.100" }} />

        {/* --- MAIN ACTIVE PANEL CONTAINER --- */}
        <Box flex="1">
          {activeMenu === "requests" && (
            <WorkshopRequestsPanel
              repairRequests={repairRequests}
              loading={loading}
              onProvideEstimation={handleProvideEstimation}
            />
          )}

          {activeMenu === "repairs" && (
            <WorkshopRepairsPanel
              repairs={repairs}
              loading={loading}
              onStartRepair={handleStartRepair}
              onCompleteRepair={handleCompleteRepair}
            />
          )}

          {activeMenu === "documents" && (
            <WorkshopDocumentsPanel
              documents={documents}
              loading={loading}
              onUploadDocument={handleUploadDocument}
              onDeleteDocument={handleDeleteDocument}
            />
          )}

          {activeMenu === "reviews" && (
            <WorkshopReviewsPanel
              reviews={reviews}
              stats={stats}
              loading={loading}
            />
          )}

          {activeMenu === "profile" && (
            <WorkshopProfilePanel
              displayName={profileDisplayName}
              setDisplayName={setProfileDisplayName}
              email={profileEmail}
              setEmail={setProfileEmail}
              phone={profilePhone}
              setPhone={setProfilePhone}
              city={profileCity}
              setCity={setProfileCity}
              address={profileAddress}
              setAddress={setProfileAddress}
              description={profileDescription}
              setDescription={setProfileDescription}
              submitting={profileSubmitting}
              onSubmit={handleProfileSubmit}
            />
          )}
        </Box>
      </Box>
    </Box>
  );
};

export default WorkshopDashboard;