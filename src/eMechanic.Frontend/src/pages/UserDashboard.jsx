import {
  Box,
  Container,
  Flex,
  HStack,
  Button,
  Image,
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
  DialogBackdrop,
  DialogRoot,
  DialogContent,
  DialogHeader,
  DialogBody,
  DialogFooter,
  DialogTitle,
  DialogCloseTrigger,
  DialogActionTrigger,
  Skeleton,
  Field,
  NativeSelect,
  Textarea,
  Spinner,
} from "@chakra-ui/react";
import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import {
  Search,
  MapPin,
  Star,
  Car,
  Clock,
  Plus,
  Wrench,
  Calendar,
  Filter,
  Phone,
  Mail,
  Trash2,
  Check,
  X,
  LogOut,
  MessageSquare,
  FileText,
  CreditCard,
  Settings,
  User,
  Upload,
  History,
  Sparkles,
} from "lucide-react";
import { useAuth } from "../context/AuthContext";
import { toaster } from "@/components/ui/toaster";
import {
  getAll as getVehicles,
  create as createVehicle,
  update as updateVehicle,
  deleteVehicle,
  getTimeline as getVehicleTimeline,
  getDocuments as getVehicleDocuments,
  uploadDocument as uploadVehicleDocument,
  downloadDocument as downloadVehicleDocument,
  deleteDocument as deleteVehicleDocument,
} from "../api/vehicles";
import {
  getWorkshops,
  getWorkshopDocuments,
  getReviews,
  upsertReview,
  deleteReview,
  getReviewStats,
} from "../api/workshops";
import {
  create as createRepairRequest,
  getByVehicleId as getRepairRequestsByVehicleId,
  acceptEstimation,
  rejectEstimation,
  getSummary as getRepairSummary,
} from "../api/repairRequests";
import { getRepairs } from "../api/repairs";
import {
  updateProfile,
  getRepairPreferences,
  updateRepairPreferences,
} from "../api/user";
import { initializePayment } from "../api/payments";

import GaragePanel from "../components/dashboard/GaragePanel";
import WorkshopsPanel from "../components/dashboard/WorkshopsPanel";
import RequestsPanel from "../components/dashboard/RequestsPanel";
import RepairsPanel from "../components/dashboard/RepairsPanel";
import PreferencesPanel from "../components/dashboard/PreferencesPanel";
import ProfilePanel from "../components/dashboard/ProfilePanel";

// --- FORMAT API ERROR HELPER ---
const formatErrorMsg = (err, fallback) => {
  if (err.response?.data?.errors) {
    const errorDetails = Object.values(err.response.data.errors)
      .flat()
      .join(", ");
    return errorDetails || fallback;
  }
  return (
    err.response?.data?.detail ||
    err.response?.data?.title ||
    err.message ||
    fallback
  );
};

// --- VEHICLE CARD COMPONENT ---
const VehicleCard = ({ vehicle, onEdit, onDelete }) => {
  const getFuelTypeString = (type) => {
    switch (type) {
      case 1:
        return "Benzyna";
      case 2:
        return "Diesel";
      case 3:
        return "LPG";
      case 4:
        return "Elektryczny";
      case 5:
        return "Hybryda";
      case 6:
        return "Wodór";
      default:
        return "Inny";
    }
  };

  const getBodyTypeString = (type) => {
    switch (type) {
      case 1:
        return "Sedan";
      case 2:
        return "Hatchback";
      case 3:
        return "Kombi";
      case 4:
        return "SUV";
      case 5:
        return "Coupe";
      case 6:
        return "Kabriolet";
      case 7:
        return "Minivan";
      case 8:
        return "Pickup";
      case 9:
        return "Van";
      default:
        return "Inny";
    }
  };

  return (
    <Box
      p={5}
      bg="white"
      _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
      rounded="2xl"
      borderWidth="1px"
      borderColor="gray.200"
      shadow="md"
      display="flex"
      flexDirection="column"
      gap={3}
      position="relative"
      transition="all 0.2s"
      _hover={{ transform: "translateY(-2px)", shadow: "lg" }}
    >
      <Flex justify="space-between" align="flex-start">
        <VStack align="flex-start" gap={0}>
          <Heading size="md" fontWeight="bold" _dark={{ color: "white" }}>
            {vehicle.manufacturer} {vehicle.model}
          </Heading>
          <Text fontSize="xs" color="gray.500" _dark={{ color: "gray.400" }}>
            VIN: {vehicle.vin}
          </Text>
        </VStack>
        <Badge
          colorPalette="brand"
          variant="solid"
          rounded="md"
          px={2}
          py={0.5}
        >
          {vehicle.licensePlate}
        </Badge>
      </Flex>

      <Separator
        borderColor="gray.100"
        _dark={{ borderColor: "whiteAlpha.100" }}
      />

      <SimpleGrid columns={2} gap={2} fontSize="sm">
        <VStack align="flex-start" gap={0}>
          <Text color="gray.400" fontSize="xs">
            Rok
          </Text>
          <Text
            fontWeight="semibold"
            color="gray.700"
            _dark={{ color: "gray.200" }}
          >
            {vehicle.productionYear}
          </Text>
        </VStack>
        <VStack align="flex-start" gap={0}>
          <Text color="gray.400" fontSize="xs">
            Przebieg
          </Text>
          <Text
            fontWeight="semibold"
            color="gray.700"
            _dark={{ color: "gray.200" }}
          >
            {vehicle.mileageValue.toLocaleString()}{" "}
            {vehicle.mileageUnit === 2 ? "mi" : "km"}
          </Text>
        </VStack>
        <VStack align="flex-start" gap={0}>
          <Text color="gray.400" fontSize="xs">
            Paliwo
          </Text>
          <Text
            fontWeight="semibold"
            color="gray.700"
            _dark={{ color: "gray.200" }}
          >
            {getFuelTypeString(vehicle.fuelType)}
          </Text>
        </VStack>
        <VStack align="flex-start" gap={0}>
          <Text color="gray.400" fontSize="xs">
            Nadwozie
          </Text>
          <Text
            fontWeight="semibold"
            color="gray.700"
            _dark={{ color: "gray.200" }}
          >
            {getBodyTypeString(vehicle.bodyType)}
          </Text>
        </VStack>
      </SimpleGrid>

      <Separator
        borderColor="gray.100"
        _dark={{ borderColor: "whiteAlpha.100" }}
      />

      <Flex justify="flex-end" gap={2} mt={1}>
        <Button
          size="sm"
          colorPalette="orange"
          variant="outline"
          rounded="lg"
          onClick={() => onEdit(vehicle)}
        >
          Edytuj
        </Button>
        <Button
          size="sm"
          colorPalette="red"
          variant="outline"
          rounded="lg"
          onClick={() => onDelete(vehicle.id)}
        >
          <Icon as={Trash2} boxSize={3.5} mr={1} />
          Usuń
        </Button>
      </Flex>
    </Box>
  );
};

// --- SKELETON COMPONENT ---
const CardSkeleton = () => (
  <Box
    p={5}
    bg="white"
    _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
    rounded="2xl"
    borderWidth="1px"
    borderColor="gray.200"
    shadow="md"
    display="flex"
    flexDirection="column"
    gap={3}
  >
    <Flex justify="space-between" align="flex-start">
      <VStack align="flex-start" gap={2} flex={1}>
        <Skeleton h="20px" w="60%" />
        <Skeleton h="14px" w="40%" />
      </VStack>
      <Skeleton h="24px" w="80px" rounded="md" />
    </Flex>
    <Separator
      borderColor="gray.100"
      _dark={{ borderColor: "whiteAlpha.100" }}
    />
    <SimpleGrid columns={2} gap={2}>
      {[...Array(4)].map((_, i) => (
        <VStack key={i} align="flex-start" gap={1}>
          <Skeleton h="12px" w="40%" />
          <Skeleton h="16px" w="70%" />
        </VStack>
      ))}
    </SimpleGrid>
    <Separator
      borderColor="gray.100"
      _dark={{ borderColor: "whiteAlpha.100" }}
    />
    <Flex justify="flex-end" mt={1}>
      <Skeleton h="32px" w="80px" rounded="lg" />
    </Flex>
  </Box>
);

// --- FORMAT TIMELINE EVENT HELPER ---
const formatTimelineEvent = (event) => {
  let parsedData = {};
  try {
    parsedData = JSON.parse(event.data);
  } catch (e) {
    return { title: event.eventType, description: event.data, color: "gray" };
  }

  switch (event.eventType) {
    case "VehicleCreatedDomainEvent":
      return {
        title: "Pojazd zarejestrowany",
        description: `Dodano pojazd ${parsedData.Manufacturer || ""} ${parsedData.Model || ""} do systemu z początkowym przebiegiem ${parsedData.Mileage || 0} ${parsedData.MileageUnit === 2 ? "mi" : "km"}.`,
        color: "blue",
      };
    case "VehicleMileageChangedDomainEvent":
      return {
        title: "Aktualizacja przebiegu",
        description: `Zmieniono przebieg pojazdu na ${parsedData.Mileage.NewValue || 0} ${parsedData.MileageUnit === 2 ? "mi" : "km"}.`,
        color: "orange",
      };
    case "VehicleLicensePlateChangedDomainEvent":
      return {
        title: "Zmiana numeru rejestracyjnego",
        description: `Zaktualizowano numer rejestracyjny na: ${parsedData.LicensePlate || parsedData.Value || ""}.`,
        color: "purple",
      };
    case "RepairRequestCreatedTimelineEvent":
    case "RepairRequestCreatedDomainEvent":
      return {
        title: "Złożono zgłoszenie naprawy",
        description: `Wysłano nowe zgłoszenie naprawy. Opis usterki: "${parsedData.Description || ""}".`,
        color: "yellow",
      };
    case "RepairCreatedDomainEvent":
      return {
        title: "Utworzono zlecenie naprawy",
        description: `Zlecenie zostało zaakceptowane. Szacowany koszt: ${parsedData.EstimatedCost || 0} ${parsedData.Currency || "PLN"}.`,
        color: "teal",
      };
    case "RepairStartedDomainEvent":
      return {
        title: "Rozpoczęcie prac",
        description: `Mechanik rozpoczął prace naprawcze nad pojazdem.`,
        color: "cyan",
      };
    case "RepairCompletedDomainEvent":
      return {
        title: "Naprawa zakończona",
        description: `Prace zostały ukończone. Koszt końcowy: ${parsedData.FinalCost || parsedData.EstimatedCost || 0} ${parsedData.FinalCostCurrency || parsedData.Currency || "PLN"}.`,
        color: "green",
      };
    case "RepairPaidDomainEvent":
      return {
        title: "Opłacono naprawę",
        description: `Zarejestrowano płatność za naprawę na kwotę ${parsedData.Amount || 0} ${parsedData.Currency || "PLN"}.`,
        color: "green",
      };
    case "VehicleDocumentAddedTimelineEvent":
    case "VehicleDocumentAddedDomainEvent":
      return {
        title: "Dodano dokument",
        description: `Załączono nowy dokument do pojazdu: ${parsedData.DocumentName || parsedData.Name || ""}.`,
        color: "blue",
      };
    case "VehicleDocumentDeletedTimelineEvent":
    case "VehicleDocumentDeletedDomainEvent":
      return {
        title: "Usunięto dokument",
        description: `Usunięto dokument powiązany z pojazdem: ${parsedData.FileName || parsedData.Name || ""}.`,
        color: "red",
      };
    case "VehicleVinChangedDomainEvent":
      return {
        title: "Zmiana numeru VIN",
        description: `Zaktualizowano numer VIN na: ${parsedData.Vin || parsedData.Value || ""}.`,
        color: "purple",
      };
    case "VehicleEngineCapacityChangedDomainEvent":
      return {
        title: "Zmiana pojemności silnika",
        description: `Zaktualizowano pojemność silnika na ${parsedData.EngineCapacity || parsedData.Value || ""}l.`,
        color: "purple",
      };
    case "VehicleHorsePowerChangedDomainEvent":
      return {
        title: "Zmiana mocy silnika",
        description: `Zaktualizowano moc silnika na ${parsedData.HorsePower || parsedData.Value || ""} KM.`,
        color: "purple",
      };
    case "VehicleManufacturerChangedDomainEvent":
      return {
        title: "Zmiana producenta",
        description: `Zmieniono producenta na: ${parsedData.Manufacturer || parsedData.Value || ""}.`,
        color: "purple",
      };
    case "VehicleModelChangedDomainEvent":
      return {
        title: "Zmiana modelu",
        description: `Zmieniono model na: ${parsedData.Model || parsedData.Value || ""}.`,
        color: "purple",
      };
    case "VehicleProductionYearChangedDomainEvent":
      return {
        title: "Zmiana roku produkcji",
        description: `Zmieniono rok produkcji na: ${parsedData.ProductionYear || parsedData.Value || ""}.`,
        color: "purple",
      };
    default: {
      let friendlyTitle = event.eventType
        .replace("DomainEvent", "")
        .replace("TimelineEvent", "")
        .replace("Vehicle", "")
        .replace("Changed", " - zmiana");
      friendlyTitle = friendlyTitle.replace(/([A-Z])/g, " $1").trim();
      const details = Object.entries(parsedData)
        .map(([key, val]) => `${key}: ${val}`)
        .join(", ");
      return {
        title: friendlyTitle || event.eventType,
        description: details || event.data,
        color: "gray",
      };
    }
  }
};

// --- MAIN PORTAL COMPONENT ---
const UserDashboard = ({
  activeMenu: propActiveMenu,
  setActiveMenu: propSetActiveMenu,
}) => {
  const { user, logout, refreshUser } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [localActiveMenu, localSetActiveMenu] = useState("garage");
  const activeMenu =
    propActiveMenu !== undefined ? propActiveMenu : localActiveMenu;
  const setActiveMenu =
    propSetActiveMenu !== undefined ? propSetActiveMenu : localSetActiveMenu;

  // Unified states
  const [vehicles, setVehicles] = useState([]);
  const [repairRequests, setRepairRequests] = useState([]);
  const [workshops, setWorkshops] = useState([]);

  // Loading states
  const [loadingVehicles, setLoadingVehicles] = useState(true);
  const [loadingRequests, setLoadingRequests] = useState(true);
  const [loadingWorkshops, setLoadingWorkshops] = useState(true);

  // Repairs states
  const [repairs, setRepairs] = useState([]);
  const [loadingRepairs, setLoadingRepairs] = useState(false);
  const [isProcessingPayment, setIsProcessingPayment] = useState({});

  // Document states
  const [isDocumentsOpen, setIsDocumentsOpen] = useState(false);
  const [vehicleDocuments, setVehicleDocuments] = useState([]);
  const [loadingDocuments, setLoadingDocuments] = useState(false);
  const [selectedDocumentsVehicle, setSelectedDocumentsVehicle] =
    useState(null);
  const [selectedDocumentType, setSelectedDocumentType] = useState("1"); // Invoice
  const [selectedDocumentFile, setSelectedDocumentFile] = useState(null);
  const [isUploadingDocument, setIsUploadingDocument] = useState(false);

  // Repair Preferences states
  const [partsPreference, setPartsPreference] = useState("2"); // Balanced
  const [timelinePreference, setTimelinePreference] = useState("1"); // Standard
  const [isSubmittingPreferences, setIsSubmittingPreferences] = useState(false);
  const [loadingPreferences, setLoadingPreferences] = useState(false);

  // Profile states
  const [profileFirstName, setProfileFirstName] = useState("");
  const [profileLastName, setProfileLastName] = useState("");
  const [profileEmail, setProfileEmail] = useState("");
  const [profilePhoneNumber, setProfilePhoneNumber] = useState("");
  const [isSubmittingProfile, setIsSubmittingProfile] = useState(false);

  // Workshops map state
  const [allWorkshopsMap, setAllWorkshopsMap] = useState({});

  // Form / Modal states
  const [isAddVehicleOpen, setIsAddVehicleOpen] = useState(false);
  const [isEditVehicleOpen, setIsEditVehicleOpen] = useState(false);
  const [isRepairRequestOpen, setIsRepairRequestOpen] = useState(false);
  const [isReviewsOpen, setIsReviewsOpen] = useState(false);

  // Vehicle Timeline states
  const [isTimelineOpen, setIsTimelineOpen] = useState(false);
  const [vehicleTimeline, setVehicleTimeline] = useState([]);
  const [loadingTimeline, setLoadingTimeline] = useState(false);
  const [selectedTimelineVehicle, setSelectedTimelineVehicle] = useState(null);

  // AI summary states
  const [repairSummaries, setRepairSummaries] = useState({});
  const [loadingSummaries, setLoadingSummaries] = useState({});

  // Selected items for modal operations
  const [selectedWorkshop, setSelectedWorkshop] = useState(null);
  const [editingVehicle, setEditingVehicle] = useState(null);

  // Rejection reasons state
  const [rejectingRequestId, setRejectingRequestId] = useState(null);
  const [rejectInputReason, setRejectInputReason] = useState("");

  // Form inputs: Add & Edit Vehicle
  const [vin, setVin] = useState("");
  const [manufacturer, setManufacturer] = useState("");
  const [model, setModel] = useState("");
  const [productionYear, setProductionYear] = useState("");
  const [mileage, setMileage] = useState("");
  const [licensePlate, setLicensePlate] = useState("");
  const [fuelType, setFuelType] = useState("1");
  const [bodyType, setBodyType] = useState("1");
  const [engineCapacity, setEngineCapacity] = useState("");
  const [horsePower, setHorsePower] = useState("120");
  const [vehicleType, setVehicleType] = useState("1");
  const [mileageUnit, setMileageUnit] = useState("1");
  const [isSubmittingVehicle, setIsSubmittingVehicle] = useState(false);

  // Form inputs: Create Repair Request
  const [selectedVehicleId, setSelectedVehicleId] = useState("");
  const [repairDescription, setRepairDescription] = useState("");
  const [isSubmittingRequest, setIsSubmittingRequest] = useState(false);

  // Form inputs: Workshop Review
  const [reviewRating, setReviewRating] = useState("5");
  const [reviewComment, setReviewComment] = useState("");
  const [workshopReviews, setWorkshopReviews] = useState([]);
  const [workshopStats, setWorkshopStats] = useState(null);
  const [isSubmittingReview, setIsSubmittingReview] = useState(false);

  // Filters / Search
  const [searchVehicle, setSearchVehicle] = useState("");
  const [searchWorkshop, setSearchWorkshop] = useState("");
  const [workshopPage, setWorkshopPage] = useState(1);
  const [workshopTotalPages, setWorkshopTotalPages] = useState(1);
  const [workshopTotalCount, setWorkshopTotalCount] = useState(0);

  // Fetch repairs list
  const fetchRepairsList = async () => {
    setLoadingRepairs(true);
    try {
      const data = await getRepairs({ PageNumber: 1, PageSize: 50 });
      setRepairs(data.items || []);
    } catch (err) {
      console.error("Failed to fetch repairs:", err);
      toaster.create({
        title: "Błąd pobierania",
        description: formatErrorMsg(err, "Nie udało się pobrać listy napraw."),
        type: "error",
      });
    } finally {
      setLoadingRepairs(false);
    }
  };

  // Initialize payment for a repair
  const handlePayment = async (repairId) => {
    setIsProcessingPayment((prev) => ({ ...prev, [repairId]: true }));
    try {
      const payload = {
        referenceId: repairId,
        type: 1, // Repair/Service
        successUrl: window.location.origin + "/home?payment=success",
        cancelUrl: window.location.origin + "/home?payment=cancel",
      };
      const result = await initializePayment(payload);
      if (result && result.checkoutUrl) {
        toaster.create({
          title: "Inicjowanie płatności",
          description: "Przekierowanie do bramki płatności...",
          type: "info",
        });
        window.location.href = result.checkoutUrl;
      } else {
        throw new Error("Missing checkout URL");
      }
    } catch (err) {
      console.error("Payment initialization failed:", err);
      toaster.create({
        title: "Błąd płatności",
        description: formatErrorMsg(
          err,
          "Nie udało się zainicjować płatności.",
        ),
        type: "error",
      });
    } finally {
      setIsProcessingPayment((prev) => ({ ...prev, [repairId]: false }));
    }
  };

  // Open Documents Modal and fetch data
  const openDocumentsModal = async (vehicle) => {
    setSelectedDocumentsVehicle(vehicle);
    setIsDocumentsOpen(true);
    setLoadingDocuments(true);
    setSelectedDocumentFile(null);
    try {
      const data = await getVehicleDocuments(vehicle.id, {
        PageNumber: 1,
        PageSize: 100,
      });
      setVehicleDocuments(data.items || []);
    } catch (err) {
      console.error("Failed to fetch vehicle documents:", err);
      toaster.create({
        title: "Błąd",
        description: formatErrorMsg(
          err,
          "Nie udało się pobrać dokumentów pojazdu.",
        ),
        type: "error",
      });
      setVehicleDocuments([]);
    } finally {
      setLoadingDocuments(false);
    }
  };

  // Handle Document Upload
  const handleUploadDocumentSubmit = async (e) => {
    e.preventDefault();
    if (!selectedDocumentFile) {
      toaster.create({
        title: "Błąd walidacji",
        description: "Wybierz plik do przesłania.",
        type: "error",
      });
      return;
    }

    setIsUploadingDocument(true);
    try {
      const formData = new FormData();
      formData.append("file", selectedDocumentFile);
      formData.append("documentType", parseInt(selectedDocumentType, 10));

      await uploadVehicleDocument(selectedDocumentsVehicle.id, formData);
      toaster.create({
        title: "Sukces",
        description: "Dokument został pomyślnie przesłany.",
        type: "success",
      });

      // Refresh documents list
      const data = await getVehicleDocuments(selectedDocumentsVehicle.id, {
        PageNumber: 1,
        PageSize: 100,
      });
      setVehicleDocuments(data.items || []);
      setSelectedDocumentFile(null);
      fetchVehiclesList();
    } catch (err) {
      console.error("Document upload failed:", err);
      toaster.create({
        title: "Błąd zapisu",
        description: formatErrorMsg(err, "Nie udało się przesłać dokumentu."),
        type: "error",
      });
    } finally {
      setIsUploadingDocument(false);
    }
  };

  // Handle Document Download via Blob
  const handleDownloadDocument = async (vehicleId, docId, fileName) => {
    try {
      const blob = await downloadVehicleDocument(vehicleId, docId);
      const url = window.URL.createObjectURL(new Blob([blob]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", fileName);
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
    } catch (err) {
      console.error("Failed to download document:", err);
      toaster.create({
        title: "Błąd pobierania",
        description: "Nie udało się pobrać pliku dokumentu.",
        type: "error",
      });
    }
  };

  // Handle Document Deletion
  const handleDeleteDocument = async (vehicleId, docId) => {
    try {
      await deleteVehicleDocument(vehicleId, docId);
      toaster.create({
        title: "Sukces",
        description: "Dokument został pomyślnie usunięty.",
        type: "success",
      });

      // Refresh documents list in modal
      const data = await getVehicleDocuments(vehicleId, {
        PageNumber: 1,
        PageSize: 100,
      });
      setVehicleDocuments(data.items || []);

      // Refresh vehicles list so main garage card updates
      fetchVehiclesList();
    } catch (err) {
      console.error("Failed to delete document:", err);
      toaster.create({
        title: "Błąd usuwania",
        description: formatErrorMsg(err, "Nie udało się usunąć dokumentu."),
        type: "error",
      });
    }
  };

  // Fetch Repair Preferences
  const fetchRepairPreferences = async () => {
    setLoadingPreferences(true);
    try {
      const data = await getRepairPreferences();
      if (data) {
        setPartsPreference(data.partsPreference.toString());
        setTimelinePreference(data.timelinePreference.toString());
      }
    } catch (err) {
      console.error("Failed to fetch repair preferences:", err);
    } finally {
      setLoadingPreferences(false);
    }
  };

  // Save Repair Preferences
  const handleSavePreferences = async (e) => {
    e.preventDefault();
    setIsSubmittingPreferences(true);
    try {
      const payload = {
        partsPreference: parseInt(partsPreference, 10),
        timelinePreference: parseInt(timelinePreference, 10),
      };
      await updateRepairPreferences(payload);
      toaster.create({
        title: "Zapisano preferencje",
        description: "Preferencje napraw zostały pomyślnie zaktualizowane.",
        type: "success",
      });
    } catch (err) {
      console.error("Failed to save preferences:", err);
      toaster.create({
        title: "Błąd zapisu",
        description: formatErrorMsg(err, "Nie udało się zapisać preferencji."),
        type: "error",
      });
    } finally {
      setIsSubmittingPreferences(false);
    }
  };

  // Save User Profile
  const handleSaveProfile = async (e) => {
    e.preventDefault();
    if (
      !profileFirstName.trim() ||
      !profileLastName.trim() ||
      !profileEmail.trim()
    ) {
      toaster.create({
        title: "Błąd walidacji",
        description:
          "Wypełnij wszystkie wymagane pola (Imię, Nazwisko, Email).",
        type: "error",
      });
      return;
    }

    setIsSubmittingProfile(true);
    try {
      const payload = {
        firstName: profileFirstName.trim(),
        lastName: profileLastName.trim(),
        email: profileEmail.trim(),
        phoneNumber: profilePhoneNumber.trim() || null,
      };
      await updateProfile(payload);
      await refreshUser();
      toaster.create({
        title: "Zaktualizowano profil",
        description: "Dane Twojego profilu zostały pomyślnie zaktualizowane.",
        type: "success",
      });
    } catch (err) {
      console.error("Profile update failed:", err);
      toaster.create({
        title: "Błąd zapisu",
        description: formatErrorMsg(
          err,
          "Nie udało się zaktualizować profilu.",
        ),
        type: "error",
      });
    } finally {
      setIsSubmittingProfile(false);
    }
  };

  // Fetch all workshops map
  const fetchAllWorkshopsMap = async () => {
    try {
      const data = await getWorkshops({ PageNumber: 1, PageSize: 100 });
      const map = {};
      if (data.items) {
        data.items.forEach((w) => {
          map[w.id] = w;
        });
      }
      setAllWorkshopsMap(map);
    } catch (e) {
      console.error("Failed to load workshops map:", e);
    }
  };

  // Fetch vehicles list
  const fetchVehiclesList = async () => {
    setLoadingVehicles(true);
    try {
      const data = await getVehicles({ PageNumber: 1, PageSize: 50 });
      const vehiclesData = data.items || [];
      const vehiclesWithDocs = await Promise.all(
        vehiclesData.map(async (v) => {
          try {
            const docData = await getVehicleDocuments(v.id, {
              PageNumber: 1,
              PageSize: 50,
            });
            return { ...v, documents: docData.items || [] };
          } catch (err) {
            console.error(
              `Failed to fetch documents for vehicle ${v.id}:`,
              err,
            );
            return { ...v, documents: [] };
          }
        }),
      );
      setVehicles(vehiclesWithDocs);
      return vehiclesWithDocs;
    } catch (err) {
      console.error("Failed to fetch vehicles", err);
      return [];
    } finally {
      setLoadingVehicles(false);
    }
  };

  // Open Vehicle Timeline modal and fetch data
  const openVehicleTimelineModal = async (vehicle) => {
    setSelectedTimelineVehicle(vehicle);
    setIsTimelineOpen(true);
    setLoadingTimeline(true);
    try {
      const data = await getVehicleTimeline(vehicle.id, {
        PageNumber: 1,
        PageSize: 100,
      });
      setVehicleTimeline(data.items || []);
    } catch (err) {
      console.error("Failed to fetch vehicle timeline:", err);
      toaster.create({
        title: "Błąd pobierania historii",
        description: formatErrorMsg(
          err,
          "Nie udało się pobrać historii pojazdu.",
        ),
        type: "error",
      });
      setVehicleTimeline([]);
    } finally {
      setLoadingTimeline(false);
    }
  };

  // Fetch AI Summary for a repair request
  const fetchAiSummary = async (requestId) => {
    setLoadingSummaries((prev) => ({ ...prev, [requestId]: true }));
    try {
      const summaryText = await getRepairSummary(requestId);
      setRepairSummaries((prev) => ({ ...prev, [requestId]: summaryText }));
      toaster.create({
        title: "Wygenerowano podsumowanie AI",
        description:
          "Podsumowanie zlecenia zostało pomyślnie wygenerowane przez AI.",
        type: "success",
      });
    } catch (err) {
      console.error("Failed to fetch AI summary:", err);
      toaster.create({
        title: "Błąd",
        description: formatErrorMsg(
          err,
          "Nie udało się wygenerować podsumowania AI.",
        ),
        type: "error",
      });
    } finally {
      setLoadingSummaries((prev) => ({ ...prev, [requestId]: false }));
    }
  };

  // Fetch unified repair requests
  const fetchRequestsList = async (vehiclesList) => {
    setLoadingRequests(true);
    const list = vehiclesList || vehicles;
    if (!list || list.length === 0) {
      setRepairRequests([]);
      setLoadingRequests(false);
      return;
    }

    const allRequests = [];
    for (const vehicle of list) {
      try {
        const data = await getRepairRequestsByVehicleId(vehicle.id, {
          PageNumber: 1,
          PageSize: 50,
        });
        const items = (data.items || []).map((r) => ({ ...r, vehicle }));
        allRequests.push(...items);
      } catch (e) {
        console.error("Failed to fetch requests for vehicle:", vehicle.id, e);
      }
    }
    // Sort by creation date descending
    allRequests.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
    setRepairRequests(allRequests);
    setLoadingRequests(false);
  };

  // Fetch workshops list
  const fetchWorkshopsList = async () => {
    setLoadingWorkshops(true);
    try {
      const params = {
        PageNumber: workshopPage,
        PageSize: 6,
        SearchPhrase: searchWorkshop || undefined,
      };
      const data = await getWorkshops(params);
      setWorkshops(data.items || []);
      setWorkshopTotalPages(data.totalPages || 1);
      setWorkshopTotalCount(data.totalCount || 0);
    } catch (e) {
      console.error("Failed to fetch workshops", e);
    } finally {
      setLoadingWorkshops(false);
    }
  };

  // Initial triggers
  useEffect(() => {
    fetchAllWorkshopsMap();
    if (activeMenu === "garage") {
      fetchVehiclesList().then((list) => fetchRequestsList(list));
    } else if (activeMenu === "workshops") {
      fetchWorkshopsList();
    } else if (activeMenu === "requests") {
      fetchVehiclesList().then((list) => fetchRequestsList(list));
    } else if (activeMenu === "repairs") {
      fetchVehiclesList().then(() => fetchRepairsList());
    } else if (activeMenu === "preferences") {
      fetchRepairPreferences();
    } else if (activeMenu === "profile" && user) {
      setProfileFirstName(user.firstName || "");
      setProfileLastName(user.lastName || "");
      setProfileEmail(user.email || "");
      setProfilePhoneNumber(user.phoneNumber || "");
    }
  }, [activeMenu, workshopPage, user]);

  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const paymentParam = params.get("payment");
    if (paymentParam === "success") {
      toaster.create({
        title: "Płatność udana",
        description:
          "Dziękujemy! Płatność za naprawę została zrealizowana pomyślnie.",
        type: "success",
      });
      navigate("/home", { replace: true });
      fetchRepairsList();
    } else if (paymentParam === "cancel") {
      toaster.create({
        title: "Płatność anulowana",
        description: "Proces płatności został anulowany.",
        type: "warning",
      });
      navigate("/home", { replace: true });
    }
  }, [location.search]);

  // Handle Search Trigger for workshops
  const handleWorkshopSearch = (e) => {
    e.preventDefault();
    if (workshopPage === 1) {
      fetchWorkshopsList();
    } else {
      setWorkshopPage(1);
    }
  };

  // Delete Vehicle Action
  const handleDeleteVehicle = async (id) => {
    if (!window.confirm("Czy na pewno chcesz usunąć ten pojazd?")) return;

    try {
      await deleteVehicle(id);
      toaster.create({
        title: "Pojazd usunięty",
        description: "Pojazd został pomyślnie usunięty z garażu.",
        type: "success",
      });
      const updatedList = await fetchVehiclesList();
      fetchRequestsList(updatedList);
    } catch (err) {
      console.error(err);
      toaster.create({
        title: "Błąd",
        description: formatErrorMsg(err, "Nie udało się usunąć pojazdu."),
        type: "error",
      });
    }
  };

  // Create Vehicle Action
  const handleCreateVehicle = async (e) => {
    e.preventDefault();

    const cleanVin = vin.trim();
    const cleanPlate = licensePlate.trim();

    if (cleanVin.length !== 17) {
      toaster.create({
        title: "Błąd walidacji",
        description: "Numer VIN musi składać się z dokładnie 17 znaków.",
        type: "error",
      });
      return;
    }
    if (productionYear.trim().length !== 4) {
      toaster.create({
        title: "Błąd walidacji",
        description: "Rok produkcji musi być 4-cyfrową liczbą (np. 2018).",
        type: "error",
      });
      return;
    }
    if (
      !manufacturer.trim() ||
      !model.trim() ||
      !mileage.trim() ||
      !cleanPlate
    ) {
      toaster.create({
        title: "Błąd walidacji",
        description: "Wypełnij wszystkie wymagane pola.",
        type: "error",
      });
      return;
    }

    setIsSubmittingVehicle(true);
    try {
      const payload = {
        vin: cleanVin,
        manufacturer: manufacturer.trim(),
        model: model.trim(),
        productionYear: productionYear.trim(),
        engineCapacity: engineCapacity ? parseFloat(engineCapacity) : null,
        mileageValue: parseInt(mileage, 10),
        mileageUnit: parseInt(mileageUnit, 10),
        licensePlate: cleanPlate,
        horsePower: horsePower ? parseInt(horsePower, 10) : null,
        fuelType: parseInt(fuelType, 10),
        bodyType: parseInt(bodyType, 10),
        vehicleType: parseInt(vehicleType, 10),
      };

      await createVehicle(payload);
      toaster.create({
        title: "Pojazd dodany",
        description: "Pojazd został pomyślnie dodany do Twojego garażu.",
        type: "success",
      });
      setIsAddVehicleOpen(false);

      // Reset states
      setVin("");
      setManufacturer("");
      setModel("");
      setProductionYear("");
      setMileage("");
      setLicensePlate("");
      setFuelType("1");
      setBodyType("1");
      setEngineCapacity("");
      setHorsePower("120");
      setVehicleType("1");
      setMileageUnit("1");

      const updatedList = await fetchVehiclesList();
      fetchRequestsList(updatedList);
    } catch (err) {
      console.error(err);
      toaster.create({
        title: "Błąd zapisu",
        description: formatErrorMsg(
          err,
          "Nie udało się dodać pojazdu. Sprawdź poprawność danych.",
        ),
        type: "error",
      });
    } finally {
      setIsSubmittingVehicle(false);
    }
  };

  // Open Edit Vehicle modal
  const openEditVehicleModal = (vehicle) => {
    setEditingVehicle(vehicle);
    setVin(vehicle.vin);
    setManufacturer(vehicle.manufacturer);
    setModel(vehicle.model);
    setProductionYear(vehicle.productionYear);
    setMileage(vehicle.mileageValue.toString());
    setLicensePlate(vehicle.licensePlate);
    setFuelType(vehicle.fuelType.toString());
    setBodyType(vehicle.bodyType.toString());
    setEngineCapacity(
      vehicle.engineCapacity ? vehicle.engineCapacity.toString() : "",
    );
    setHorsePower(vehicle.horsePower ? vehicle.horsePower.toString() : "120");
    setVehicleType(vehicle.vehicleType ? vehicle.vehicleType.toString() : "1");
    setMileageUnit(vehicle.mileageUnit ? vehicle.mileageUnit.toString() : "1");
    setIsEditVehicleOpen(true);
  };

  // Update Vehicle Action
  const handleUpdateVehicle = async (e) => {
    e.preventDefault();

    const cleanVin = vin.trim();
    const cleanPlate = licensePlate.trim();

    if (cleanVin.length !== 17) {
      toaster.create({
        title: "Błąd walidacji",
        description: "Numer VIN musi składać się z dokładnie 17 znaków.",
        type: "error",
      });
      return;
    }
    if (productionYear.trim().length !== 4) {
      toaster.create({
        title: "Błąd walidacji",
        description: "Rok produkcji musi być 4-cyfrową liczbą (np. 2018).",
        type: "error",
      });
      return;
    }
    if (
      !manufacturer.trim() ||
      !model.trim() ||
      !mileage.trim() ||
      !cleanPlate
    ) {
      toaster.create({
        title: "Błąd walidacji",
        description: "Wypełnij wszystkie wymagane pola.",
        type: "error",
      });
      return;
    }

    setIsSubmittingVehicle(true);
    try {
      const payload = {
        vin: cleanVin,
        manufacturer: manufacturer.trim(),
        model: model.trim(),
        productionYear: productionYear.trim(),
        engineCapacity: engineCapacity ? parseFloat(engineCapacity) : null,
        mileageValue: parseInt(mileage, 10),
        mileageUnit: parseInt(mileageUnit, 10),
        licensePlate: cleanPlate,
        horsePower: horsePower ? parseInt(horsePower, 10) : null,
        fuelType: parseInt(fuelType, 10),
        bodyType: parseInt(bodyType, 10),
        vehicleType: parseInt(vehicleType, 10),
      };

      await updateVehicle(editingVehicle.id, payload);
      toaster.create({
        title: "Pojazd zaktualizowany",
        description: "Pojazd został pomyślnie zaktualizowany.",
        type: "success",
      });
      setIsEditVehicleOpen(false);

      // Reset states
      setVin("");
      setManufacturer("");
      setModel("");
      setProductionYear("");
      setMileage("");
      setLicensePlate("");
      setFuelType("1");
      setBodyType("1");
      setEngineCapacity("");
      setHorsePower("120");
      setVehicleType("1");
      setMileageUnit("1");
      setEditingVehicle(null);

      const updatedList = await fetchVehiclesList();
      fetchRequestsList(updatedList);
    } catch (err) {
      console.error(err);
      toaster.create({
        title: "Błąd zapisu",
        description: formatErrorMsg(
          err,
          "Nie udało się zaktualizować pojazdu.",
        ),
        type: "error",
      });
    } finally {
      setIsSubmittingVehicle(false);
    }
  };

  // Open Repair Request modal
  const openRepairRequestModal = (workshop) => {
    if (vehicles.length === 0) {
      toaster.create({
        title: "Brak pojazdów",
        description: "Proszę najpierw dodać pojazd w sekcji Garaż.",
        type: "error",
      });
      return;
    }
    setSelectedWorkshop(workshop);
    setSelectedVehicleId(vehicles[0].id);
    setRepairDescription("");
    setIsRepairRequestOpen(true);
  };

  // Create Repair Request Action
  const handleCreateRepairRequest = async (e) => {
    e.preventDefault();
    if (!repairDescription.trim()) {
      toaster.create({
        title: "Błąd walidacji",
        description: "Opisz usterkę.",
        type: "error",
      });
      return;
    }

    setIsSubmittingRequest(true);
    try {
      await createRepairRequest(
        selectedVehicleId,
        selectedWorkshop.id,
        repairDescription.trim(),
      );
      toaster.create({
        title: "Zlecenie wysłane",
        description: "Zlecenie naprawy zostało pomyślnie wysłane do warsztatu.",
        type: "success",
      });
      setIsRepairRequestOpen(false);
      fetchRequestsList();
    } catch (err) {
      console.error(err);
      toaster.create({
        title: "Błąd",
        description: formatErrorMsg(err, "Nie udało się wysłać zlecenia."),
        type: "error",
      });
    } finally {
      setIsSubmittingRequest(false);
    }
  };

  // Load Workshop Reviews & Stats
  const openReviewsModal = async (workshop) => {
    setSelectedWorkshop(workshop);
    setIsReviewsOpen(true);
    setWorkshopReviews([]);
    setWorkshopStats(null);
    setReviewComment("");
    setReviewRating("5");

    try {
      // Fetch reviews
      const reviewsData = await getReviews(workshop.id, {
        PageNumber: 1,
        PageSize: 50,
      });
      const items = reviewsData.items || [];
      setWorkshopReviews(items);

      // Pre-populate user's existing review if any
      const myReview = items.find((r) => r.userId === user?.id);
      if (myReview) {
        setReviewComment(myReview.comment || "");
        setReviewRating(myReview.rating.toString());
      }

      // Fetch stats
      const statsData = await getReviewStats(workshop.id);
      setWorkshopStats(statsData);
    } catch (e) {
      console.error("Failed to load reviews data:", e);
    }
  };

  // Submit/Update Review Action
  const handleUpsertReview = async (e) => {
    e.preventDefault();
    setIsSubmittingReview(true);
    try {
      await upsertReview(
        selectedWorkshop.id,
        parseInt(reviewRating, 10),
        reviewComment.trim() || null,
      );
      toaster.create({
        title: "Opinia zapisana",
        description: "Dziękujemy za wystawienie opinii.",
        type: "success",
      });

      // Reload reviews
      const reviewsData = await getReviews(selectedWorkshop.id, {
        PageNumber: 1,
        PageSize: 50,
      });
      setWorkshopReviews(reviewsData.items || []);

      const statsData = await getReviewStats(selectedWorkshop.id);
      setWorkshopStats(statsData);

      setReviewComment("");
    } catch (err) {
      console.error(err);
      toaster.create({
        title: "Błąd zapisu",
        description: formatErrorMsg(err, "Nie udało się zapisać opinii."),
        type: "error",
      });
    } finally {
      setIsSubmittingReview(false);
    }
  };

  // Delete Review Action
  const handleDeleteReview = async () => {
    if (!window.confirm("Czy na pewno chcesz usunąć swoją opinię?")) return;
    try {
      await deleteReview(selectedWorkshop.id);
      toaster.create({
        title: "Opinia usunięta",
        description: "Twoja opinia została pomyślnie usunięta.",
        type: "success",
      });

      // Reload reviews
      const reviewsData = await getReviews(selectedWorkshop.id, {
        PageNumber: 1,
        PageSize: 50,
      });
      setWorkshopReviews(reviewsData.items || []);

      const statsData = await getReviewStats(selectedWorkshop.id);
      setWorkshopStats(statsData);
    } catch (err) {
      console.error(err);
      toaster.create({
        title: "Błąd",
        description: formatErrorMsg(err, "Nie udało się usunąć opinii."),
        type: "error",
      });
    }
  };

  // Accept Estimation Action
  const handleAcceptEstimation = async (requestId) => {
    try {
      await acceptEstimation(requestId);
      toaster.create({
        title: "Wycena zaakceptowana",
        description: "Wycena została pomyślnie zaakceptowana.",
        type: "success",
      });
      fetchRequestsList();
    } catch (err) {
      console.error(err);
      toaster.create({
        title: "Błąd",
        description: formatErrorMsg(err, "Nie udało się zaakceptować wyceny."),
        type: "error",
      });
    }
  };

  // Reject Estimation Action
  const handleRejectEstimationSubmit = async (requestId) => {
    if (!rejectInputReason.trim()) {
      toaster.create({
        title: "Wpisz powód",
        description: "Musisz podać powód odrzucenia wyceny.",
        type: "error",
      });
      return;
    }

    try {
      await rejectEstimation(requestId, rejectInputReason.trim());
      toaster.create({
        title: "Wycena odrzucona",
        description: "Wycena została pomyślnie odrzucona.",
        type: "success",
      });
      setRejectingRequestId(null);
      setRejectInputReason("");
      fetchRequestsList();
    } catch (err) {
      console.error(err);
      toaster.create({
        title: "Błąd",
        description: formatErrorMsg(err, "Nie udało się odrzucić wyceny."),
        type: "error",
      });
    }
  };

  // Filter vehicles
  const filteredVehicles = vehicles.filter(
    (v) =>
      v.manufacturer.toLowerCase().includes(searchVehicle.toLowerCase()) ||
      v.model.toLowerCase().includes(searchVehicle.toLowerCase()) ||
      v.licensePlate.toLowerCase().includes(searchVehicle.toLowerCase()) ||
      v.vin.toLowerCase().includes(searchVehicle.toLowerCase()),
  );

  // Get Status display details
  const getStatusDetails = (status) => {
    switch (status) {
      case 1:
        return {
          label: "Oczekuje",
          color: "orange",
          desc: "Zlecenie czeka na diagnozę i wycenę warsztatu.",
        };
      case 2:
        return {
          label: "Wyceniono",
          color: "yellow",
          desc: "Warsztat postawił diagnozę. Oczekiwanie na Twoją akceptację.",
        };
      case 3:
        return {
          label: "Zaakceptowano",
          color: "green",
          desc: "Wycena zaakceptowana. Trwają prace naprawcze.",
        };
      case 4:
        return {
          label: "Odrzucono",
          color: "red",
          desc: "Odrzuciłeś wycenę tego zlecenia.",
        };
      default:
        return { label: "Nieznany", color: "gray", desc: "" };
    }
  };

  return (
    <Box
      minH="100vh"
      w="full"
      display="flex"
      bg="gray.100"
      _dark={{ bg: "#0F172A" }}
      pt="80px"
    >
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
          {/* Menu options */}
          <VStack align="stretch" gap={2}>
            <Button
              onClick={() => setActiveMenu("garage")}
              justifyContent="flex-start"
              variant="ghost"
              h="12"
              rounded="xl"
              color={activeMenu === "garage" ? "brand.600" : "gray.500"}
              bg={activeMenu === "garage" ? "brand.50" : "transparent"}
              _dark={{
                color: activeMenu === "garage" ? "brand.300" : "gray.400",
                bg: activeMenu === "garage" ? "whiteAlpha.100" : "transparent",
              }}
              gap={3}
            >
              <Icon as={Car} boxSize={5} />
              Mój Garaż
            </Button>

            <Button
              onClick={() => setActiveMenu("workshops")}
              justifyContent="flex-start"
              variant="ghost"
              h="12"
              rounded="xl"
              color={activeMenu === "workshops" ? "brand.600" : "gray.500"}
              bg={activeMenu === "workshops" ? "brand.50" : "transparent"}
              _dark={{
                color: activeMenu === "workshops" ? "brand.300" : "gray.400",
                bg:
                  activeMenu === "workshops" ? "whiteAlpha.100" : "transparent",
              }}
              gap={3}
            >
              <Icon as={Wrench} boxSize={5} />
              Wyszukaj Warsztat
            </Button>

            <Button
              onClick={() => setActiveMenu("requests")}
              justifyContent="flex-start"
              variant="ghost"
              h="12"
              rounded="xl"
              color={activeMenu === "requests" ? "brand.600" : "gray.500"}
              bg={activeMenu === "requests" ? "brand.50" : "transparent"}
              _dark={{
                color: activeMenu === "requests" ? "brand.300" : "gray.400",
                bg:
                  activeMenu === "requests" ? "whiteAlpha.100" : "transparent",
              }}
              gap={3}
            >
              <Icon as={Clock} boxSize={5} />
              Zlecenia Napraw
            </Button>

            <Button
              onClick={() => setActiveMenu("repairs")}
              justifyContent="flex-start"
              variant="ghost"
              h="12"
              rounded="xl"
              color={activeMenu === "repairs" ? "brand.600" : "gray.500"}
              bg={activeMenu === "repairs" ? "brand.50" : "transparent"}
              _dark={{
                color: activeMenu === "repairs" ? "brand.300" : "gray.400",
                bg: activeMenu === "repairs" ? "whiteAlpha.100" : "transparent",
              }}
              gap={3}
            >
              <Icon as={CreditCard} boxSize={5} />
              Aktywne Naprawy
            </Button>

            <Button
              onClick={() => setActiveMenu("preferences")}
              justifyContent="flex-start"
              variant="ghost"
              h="12"
              rounded="xl"
              color={activeMenu === "preferences" ? "brand.600" : "gray.500"}
              bg={activeMenu === "preferences" ? "brand.50" : "transparent"}
              _dark={{
                color: activeMenu === "preferences" ? "brand.300" : "gray.400",
                bg:
                  activeMenu === "preferences"
                    ? "whiteAlpha.100"
                    : "transparent",
              }}
              gap={3}
            >
              <Icon as={Settings} boxSize={5} />
              Preferencje Napraw
            </Button>

            <Button
              onClick={() => setActiveMenu("profile")}
              justifyContent="flex-start"
              variant="ghost"
              h="12"
              rounded="xl"
              color={activeMenu === "profile" ? "brand.600" : "gray.500"}
              bg={activeMenu === "profile" ? "brand.50" : "transparent"}
              _dark={{
                color: activeMenu === "profile" ? "brand.300" : "gray.400",
                bg: activeMenu === "profile" ? "whiteAlpha.100" : "transparent",
              }}
              gap={3}
            >
              <Icon as={User} boxSize={5} />
              Mój Profil
            </Button>
          </VStack>
        </VStack>
      </Box>

      {/* --- RIGHT CONTENT AREA --- */}
      <Box flex="1" p={8} overflowY="auto" h="calc(100vh - 80px)">
        {activeMenu === "garage" && (
          <GaragePanel
            vehicles={vehicles}
            loadingVehicles={loadingVehicles}
            searchVehicle={searchVehicle}
            setSearchVehicle={setSearchVehicle}
            onAddVehicleClick={() => setIsAddVehicleOpen(true)}
            onEditVehicleClick={openEditVehicleModal}
            onDeleteVehicle={handleDeleteVehicle}
            onTimelineClick={openVehicleTimelineModal}
            onDocumentsClick={openDocumentsModal}
            repairRequests={repairRequests}
          />
        )}

        {activeMenu === "workshops" && (
          <WorkshopsPanel
            workshops={workshops}
            loadingWorkshops={loadingWorkshops}
            searchWorkshop={searchWorkshop}
            setSearchWorkshop={setSearchWorkshop}
            handleWorkshopSearch={handleWorkshopSearch}
            workshopTotalCount={workshopTotalCount}
            workshopPage={workshopPage}
            workshopTotalPages={workshopTotalPages}
            setWorkshopPage={setWorkshopPage}
            openReviewsModal={openReviewsModal}
            openRepairRequestModal={openRepairRequestModal}
          />
        )}

        {activeMenu === "requests" && (
          <RequestsPanel
            repairRequests={repairRequests}
            loadingRequests={loadingRequests}
            allWorkshopsMap={allWorkshopsMap}
            handleAcceptEstimation={handleAcceptEstimation}
            rejectingRequestId={rejectingRequestId}
            setRejectingRequestId={setRejectingRequestId}
            rejectInputReason={rejectInputReason}
            setRejectInputReason={setRejectInputReason}
            handleRejectEstimationSubmit={handleRejectEstimationSubmit}
            repairSummaries={repairSummaries}
            loadingSummaries={loadingSummaries}
            fetchAiSummary={fetchAiSummary}
          />
        )}

        {activeMenu === "repairs" && (
          <RepairsPanel
            repairs={repairs}
            loading={loadingRepairs}
            isProcessingPayment={isProcessingPayment}
            onPay={handlePayment}
            vehicles={vehicles}
            allWorkshopsMap={allWorkshopsMap}
          />
        )}

        {activeMenu === "preferences" && (
          <PreferencesPanel
            parts={partsPreference}
            setParts={setPartsPreference}
            timeline={timelinePreference}
            setTimeline={setTimelinePreference}
            loading={loadingPreferences}
            submitting={isSubmittingPreferences}
            onSubmit={handleSavePreferences}
          />
        )}

        {activeMenu === "profile" && (
          <ProfilePanel
            firstName={profileFirstName}
            setFirstName={setProfileFirstName}
            lastName={profileLastName}
            setLastName={setProfileLastName}
            email={profileEmail}
            setEmail={setProfileEmail}
            phone={profilePhoneNumber}
            setPhone={setProfilePhoneNumber}
            submitting={isSubmittingProfile}
            onSubmit={handleSaveProfile}
          />
        )}
      </Box>

      {/* ========================================================
            DIALOG: ADD NEW VEHICLE
            ======================================================== */}
      <DialogRoot
        open={isAddVehicleOpen}
        onOpenChange={(e) => setIsAddVehicleOpen(e.open)}
      >
        <DialogBackdrop />
        <DialogContent _dark={{ bg: "rgb(25, 36, 54)", color: "white" }}>
          <form onSubmit={handleCreateVehicle}>
            <DialogHeader>
              <DialogTitle fontSize="xl" fontWeight="bold">
                Dodaj Nowy Pojazd
              </DialogTitle>
            </DialogHeader>
            <DialogBody display="flex" flexDirection="column" gap={4}>
              <Field.Root required>
                <Field.Label>VIN (dokładnie 17 znaków)</Field.Label>
                <Input
                  placeholder="Wpisz numer VIN"
                  value={vin}
                  onChange={(e) => setVin(e.target.value)}
                  maxLength={17}
                  _dark={{
                    bg: "rgb(15, 23, 42)",
                    borderColor: "whiteAlpha.200",
                  }}
                />
                <Text fontSize="10px" color="gray.400">
                  VIN musi mieć dokładnie 17 znaków alfanumerycznych.
                </Text>
              </Field.Root>

              <SimpleGrid columns={2} gap={4}>
                <Field.Root required>
                  <Field.Label>Marka</Field.Label>
                  <Input
                    placeholder="np. Toyota"
                    value={manufacturer}
                    onChange={(e) => setManufacturer(e.target.value)}
                    _dark={{
                      bg: "rgb(15, 23, 42)",
                      borderColor: "whiteAlpha.200",
                    }}
                  />
                </Field.Root>

                <Field.Root required>
                  <Field.Label>Model</Field.Label>
                  <Input
                    placeholder="np. Corolla"
                    value={model}
                    onChange={(e) => setModel(e.target.value)}
                    _dark={{
                      bg: "rgb(15, 23, 42)",
                      borderColor: "whiteAlpha.200",
                    }}
                  />
                </Field.Root>
              </SimpleGrid>

              <SimpleGrid columns={2} gap={4}>
                <Field.Root required>
                  <Field.Label>Rok produkcji (4 cyfry)</Field.Label>
                  <Input
                    type="number"
                    placeholder="np. 2018"
                    value={productionYear}
                    onChange={(e) => setProductionYear(e.target.value)}
                    maxLength={4}
                    _dark={{
                      bg: "rgb(15, 23, 42)",
                      borderColor: "whiteAlpha.200",
                    }}
                  />
                </Field.Root>

                <Field.Root required>
                  <Field.Label>Numer rejestracyjny</Field.Label>
                  <Input
                    placeholder="np. PO12345"
                    value={licensePlate}
                    onChange={(e) => setLicensePlate(e.target.value)}
                    _dark={{
                      bg: "rgb(15, 23, 42)",
                      borderColor: "whiteAlpha.200",
                    }}
                  />
                </Field.Root>
              </SimpleGrid>

              <Field.Root required>
                <Field.Label>Przebieg (km)</Field.Label>
                <Input
                  type="number"
                  placeholder="np. 150000"
                  value={mileage}
                  onChange={(e) => setMileage(e.target.value)}
                  _dark={{
                    bg: "rgb(15, 23, 42)",
                    borderColor: "whiteAlpha.200",
                  }}
                />
              </Field.Root>

              <SimpleGrid columns={2} gap={4}>
                <Field.Root>
                  <Field.Label>Paliwo</Field.Label>
                  <NativeSelect.Root>
                    <NativeSelect.Field
                      value={fuelType}
                      onChange={(e) => setFuelType(e.target.value)}
                      _dark={{
                        bg: "rgb(15, 23, 42)",
                        borderColor: "whiteAlpha.200",
                      }}
                    >
                      <option value="1">Benzyna</option>
                      <option value="2">Diesel</option>
                      <option value="3">LPG</option>
                      <option value="4">Elektryczny</option>
                      <option value="5">Hybryda</option>
                      <option value="6">Wodór</option>
                    </NativeSelect.Field>
                  </NativeSelect.Root>
                </Field.Root>

                <Field.Root>
                  <Field.Label>Typ nadwozia</Field.Label>
                  <NativeSelect.Root>
                    <NativeSelect.Field
                      value={bodyType}
                      onChange={(e) => setBodyType(e.target.value)}
                      _dark={{
                        bg: "rgb(15, 23, 42)",
                        borderColor: "whiteAlpha.200",
                      }}
                    >
                      <option value="1">Sedan</option>
                      <option value="2">Hatchback</option>
                      <option value="3">Kombi</option>
                      <option value="4">SUV</option>
                      <option value="5">Coupe</option>
                      <option value="6">Kabriolet</option>
                      <option value="7">Minivan</option>
                      <option value="8">Pickup</option>
                      <option value="9">Van</option>
                    </NativeSelect.Field>
                  </NativeSelect.Root>
                </Field.Root>
              </SimpleGrid>

              <SimpleGrid columns={2} gap={4}>
                <Field.Root>
                  <Field.Label>Typ pojazdu</Field.Label>
                  <NativeSelect.Root>
                    <NativeSelect.Field
                      value={vehicleType}
                      onChange={(e) => setVehicleType(e.target.value)}
                      _dark={{
                        bg: "rgb(15, 23, 42)",
                        borderColor: "whiteAlpha.200",
                      }}
                    >
                      <option value="1">Osobowy</option>
                      <option value="2">Motocykl</option>
                    </NativeSelect.Field>
                  </NativeSelect.Root>
                </Field.Root>

                <Field.Root>
                  <Field.Label>Jednostka przebiegu</Field.Label>
                  <NativeSelect.Root>
                    <NativeSelect.Field
                      value={mileageUnit}
                      onChange={(e) => setMileageUnit(e.target.value)}
                      _dark={{
                        bg: "rgb(15, 23, 42)",
                        borderColor: "whiteAlpha.200",
                      }}
                    >
                      <option value="1">Kilometry (km)</option>
                      <option value="2">Mile (mi)</option>
                    </NativeSelect.Field>
                  </NativeSelect.Root>
                </Field.Root>
              </SimpleGrid>

              <SimpleGrid columns={2} gap={4}>
                <Field.Root>
                  <Field.Label>Pojemność silnika (L)</Field.Label>
                  <Input
                    type="number"
                    step="0.1"
                    placeholder="np. 1.6"
                    value={engineCapacity}
                    onChange={(e) => setEngineCapacity(e.target.value)}
                    _dark={{
                      bg: "rgb(15, 23, 42)",
                      borderColor: "whiteAlpha.200",
                    }}
                  />
                </Field.Root>

                <Field.Root>
                  <Field.Label>Moc silnika (KM)</Field.Label>
                  <Input
                    type="number"
                    placeholder="np. 150"
                    value={horsePower}
                    onChange={(e) => setHorsePower(e.target.value)}
                    _dark={{
                      bg: "rgb(15, 23, 42)",
                      borderColor: "whiteAlpha.200",
                    }}
                  />
                </Field.Root>
              </SimpleGrid>
            </DialogBody>
            <DialogFooter gap={2}>
              <DialogActionTrigger asChild>
                <Button variant="ghost" rounded="lg">
                  Anuluj
                </Button>
              </DialogActionTrigger>
              <Button
                type="submit"
                loading={isSubmittingVehicle}
                colorPalette="orange"
                rounded="lg"
              >
                Zapisz pojazd
              </Button>
            </DialogFooter>
          </form>
          <DialogCloseTrigger />
        </DialogContent>
      </DialogRoot>

      {/* ========================================================
            DIALOG: EDIT EXISTING VEHICLE
            ======================================================== */}
      <DialogRoot
        open={isEditVehicleOpen}
        onOpenChange={(e) => setIsEditVehicleOpen(e.open)}
      >
        <DialogBackdrop />
        <DialogContent _dark={{ bg: "rgb(25, 36, 54)", color: "white" }}>
          <form onSubmit={handleUpdateVehicle}>
            <DialogHeader>
              <DialogTitle fontSize="xl" fontWeight="bold">
                Edytuj Pojazd
              </DialogTitle>
            </DialogHeader>
            <DialogBody display="flex" flexDirection="column" gap={4}>
              <Field.Root required>
                <Field.Label>VIN (dokładnie 17 znaków)</Field.Label>
                <Input
                  placeholder="Wpisz numer VIN"
                  value={vin}
                  onChange={(e) => setVin(e.target.value)}
                  maxLength={17}
                  _dark={{
                    bg: "rgb(15, 23, 42)",
                    borderColor: "whiteAlpha.200",
                  }}
                />
                <Text fontSize="10px" color="gray.400">
                  VIN musi mieć dokładnie 17 znaków alfanumerycznych.
                </Text>
              </Field.Root>

              <SimpleGrid columns={2} gap={4}>
                <Field.Root required>
                  <Field.Label>Marka</Field.Label>
                  <Input
                    placeholder="np. Toyota"
                    value={manufacturer}
                    onChange={(e) => setManufacturer(e.target.value)}
                    _dark={{
                      bg: "rgb(15, 23, 42)",
                      borderColor: "whiteAlpha.200",
                    }}
                  />
                </Field.Root>

                <Field.Root required>
                  <Field.Label>Model</Field.Label>
                  <Input
                    placeholder="np. Corolla"
                    value={model}
                    onChange={(e) => setModel(e.target.value)}
                    _dark={{
                      bg: "rgb(15, 23, 42)",
                      borderColor: "whiteAlpha.200",
                    }}
                  />
                </Field.Root>
              </SimpleGrid>

              <SimpleGrid columns={2} gap={4}>
                <Field.Root required>
                  <Field.Label>Rok produkcji (4 cyfry)</Field.Label>
                  <Input
                    type="number"
                    placeholder="np. 2018"
                    value={productionYear}
                    onChange={(e) => setProductionYear(e.target.value)}
                    maxLength={4}
                    _dark={{
                      bg: "rgb(15, 23, 42)",
                      borderColor: "whiteAlpha.200",
                    }}
                  />
                </Field.Root>

                <Field.Root required>
                  <Field.Label>Numer rejestracyjny</Field.Label>
                  <Input
                    placeholder="np. PO12345"
                    value={licensePlate}
                    onChange={(e) => setLicensePlate(e.target.value)}
                    _dark={{
                      bg: "rgb(15, 23, 42)",
                      borderColor: "whiteAlpha.200",
                    }}
                  />
                </Field.Root>
              </SimpleGrid>

              <Field.Root required>
                <Field.Label>Przebieg (km)</Field.Label>
                <Input
                  type="number"
                  placeholder="np. 150000"
                  value={mileage}
                  onChange={(e) => setMileage(e.target.value)}
                  _dark={{
                    bg: "rgb(15, 23, 42)",
                    borderColor: "whiteAlpha.200",
                  }}
                />
              </Field.Root>

              <SimpleGrid columns={2} gap={4}>
                <Field.Root>
                  <Field.Label>Paliwo</Field.Label>
                  <NativeSelect.Root>
                    <NativeSelect.Field
                      value={fuelType}
                      onChange={(e) => setFuelType(e.target.value)}
                      _dark={{
                        bg: "rgb(15, 23, 42)",
                        borderColor: "whiteAlpha.200",
                      }}
                    >
                      <option value="1">Benzyna</option>
                      <option value="2">Diesel</option>
                      <option value="3">LPG</option>
                      <option value="4">Elektryczny</option>
                      <option value="5">Hybryda</option>
                      <option value="6">Wodór</option>
                    </NativeSelect.Field>
                  </NativeSelect.Root>
                </Field.Root>

                <Field.Root>
                  <Field.Label>Typ nadwozia</Field.Label>
                  <NativeSelect.Root>
                    <NativeSelect.Field
                      value={bodyType}
                      onChange={(e) => setBodyType(e.target.value)}
                      _dark={{
                        bg: "rgb(15, 23, 42)",
                        borderColor: "whiteAlpha.200",
                      }}
                    >
                      <option value="1">Sedan</option>
                      <option value="2">Hatchback</option>
                      <option value="3">Kombi</option>
                      <option value="4">SUV</option>
                      <option value="5">Coupe</option>
                      <option value="6">Kabriolet</option>
                      <option value="7">Minivan</option>
                      <option value="8">Pickup</option>
                      <option value="9">Van</option>
                    </NativeSelect.Field>
                  </NativeSelect.Root>
                </Field.Root>
              </SimpleGrid>

              <SimpleGrid columns={2} gap={4}>
                <Field.Root>
                  <Field.Label>Typ pojazdu</Field.Label>
                  <NativeSelect.Root>
                    <NativeSelect.Field
                      value={vehicleType}
                      onChange={(e) => setVehicleType(e.target.value)}
                      _dark={{
                        bg: "rgb(15, 23, 42)",
                        borderColor: "whiteAlpha.200",
                      }}
                    >
                      <option value="1">Osobowy</option>
                      <option value="2">Motocykl</option>
                    </NativeSelect.Field>
                  </NativeSelect.Root>
                </Field.Root>

                <Field.Root>
                  <Field.Label>Jednostka przebiegu</Field.Label>
                  <NativeSelect.Root>
                    <NativeSelect.Field
                      value={mileageUnit}
                      onChange={(e) => setMileageUnit(e.target.value)}
                      _dark={{
                        bg: "rgb(15, 23, 42)",
                        borderColor: "whiteAlpha.200",
                      }}
                    >
                      <option value="1">Kilometry (km)</option>
                      <option value="2">Mile (mi)</option>
                    </NativeSelect.Field>
                  </NativeSelect.Root>
                </Field.Root>
              </SimpleGrid>

              <SimpleGrid columns={2} gap={4}>
                <Field.Root>
                  <Field.Label>Pojemność silnika (L)</Field.Label>
                  <Input
                    type="number"
                    step="0.1"
                    placeholder="np. 1.6"
                    value={engineCapacity}
                    onChange={(e) => setEngineCapacity(e.target.value)}
                    _dark={{
                      bg: "rgb(15, 23, 42)",
                      borderColor: "whiteAlpha.200",
                    }}
                  />
                </Field.Root>

                <Field.Root>
                  <Field.Label>Moc silnika (KM)</Field.Label>
                  <Input
                    type="number"
                    placeholder="np. 150"
                    value={horsePower}
                    onChange={(e) => setHorsePower(e.target.value)}
                    _dark={{
                      bg: "rgb(15, 23, 42)",
                      borderColor: "whiteAlpha.200",
                    }}
                  />
                </Field.Root>
              </SimpleGrid>
            </DialogBody>
            <DialogFooter gap={2}>
              <DialogActionTrigger asChild>
                <Button
                  variant="ghost"
                  rounded="lg"
                  onClick={() => setEditingVehicle(null)}
                >
                  Anuluj
                </Button>
              </DialogActionTrigger>
              <Button
                type="submit"
                loading={isSubmittingVehicle}
                colorPalette="orange"
                rounded="lg"
              >
                Zapisz zmiany
              </Button>
            </DialogFooter>
          </form>
          <DialogCloseTrigger />
        </DialogContent>
      </DialogRoot>

      {/* ========================================================
            DIALOG: CREATE REPAIR REQUEST
            ======================================================== */}
      <DialogRoot
        open={isRepairRequestOpen}
        onOpenChange={(e) => setIsRepairRequestOpen(e.open)}
      >
        <DialogBackdrop />
        <DialogContent _dark={{ bg: "rgb(25, 36, 54)", color: "white" }}>
          <form onSubmit={handleCreateRepairRequest}>
            <DialogHeader>
              <DialogTitle fontSize="xl" fontWeight="bold">
                Zleć naprawę
              </DialogTitle>
              <Text fontSize="xs" color="gray.400" mt={1}>
                Warsztat: {selectedWorkshop?.displayName}
              </Text>
            </DialogHeader>
            <DialogBody display="flex" flexDirection="column" gap={4}>
              <Field.Root required>
                <Field.Label>Wybierz samochód</Field.Label>
                <NativeSelect.Root>
                  <NativeSelect.Field
                    value={selectedVehicleId}
                    onChange={(e) => setSelectedVehicleId(e.target.value)}
                    _dark={{
                      bg: "rgb(15, 23, 42)",
                      borderColor: "whiteAlpha.200",
                    }}
                  >
                    {vehicles.map((v) => (
                      <option key={v.id} value={v.id}>
                        {v.manufacturer} {v.model} ({v.licensePlate})
                      </option>
                    ))}
                  </NativeSelect.Field>
                </NativeSelect.Root>
              </Field.Root>

              <Field.Root required>
                <Field.Label>Opis usterki / Zakres prac</Field.Label>
                <Textarea
                  placeholder="Opisz co dzieje się z autem, jakie usłyszałeś objawy..."
                  value={repairDescription}
                  onChange={(e) => setRepairDescription(e.target.value)}
                  rows={5}
                  _dark={{
                    bg: "rgb(15, 23, 42)",
                    borderColor: "whiteAlpha.200",
                  }}
                />
              </Field.Root>
            </DialogBody>
            <DialogFooter gap={2}>
              <DialogActionTrigger asChild>
                <Button variant="ghost" rounded="lg">
                  Anuluj
                </Button>
              </DialogActionTrigger>
              <Button
                type="submit"
                loading={isSubmittingRequest}
                colorPalette="orange"
                rounded="lg"
              >
                Wyślij zlecenie
              </Button>
            </DialogFooter>
          </form>
          <DialogCloseTrigger />
        </DialogContent>
      </DialogRoot>

      {/* ========================================================
            DIALOG: WORKSHOP REVIEWS & ADD REVIEW
            ======================================================== */}
      <DialogRoot
        size="lg"
        open={isReviewsOpen}
        onOpenChange={(e) => setIsReviewsOpen(e.open)}
      >
        <DialogBackdrop />
        <DialogContent _dark={{ bg: "rgb(25, 36, 54)", color: "white" }}>
          <DialogHeader>
            <DialogTitle fontSize="xl" fontWeight="bold">
              Opinie o: {selectedWorkshop?.displayName}
            </DialogTitle>
            {workshopStats && (
              <HStack gap={4} mt={2}>
                <Badge
                  colorPalette="orange"
                  variant="outline"
                  rounded="md"
                  px={2}
                  py={0.5}
                >
                  Średnia ocena:{" "}
                  {parseFloat(workshopStats.averageRating || 0).toFixed(1)} /
                  5.0
                </Badge>
                <Text fontSize="xs" color="gray.400">
                  Suma ocen: {workshopStats.totalReviews || 0}
                </Text>
              </HStack>
            )}
          </DialogHeader>
          <DialogBody
            display="flex"
            flexDirection="column"
            gap={6}
            maxH="500px"
            overflowY="auto"
          >
            {/* Form to submit review */}
            <Box
              p={4}
              bg="gray.50"
              _dark={{ bg: "rgb(15, 23, 42)" }}
              rounded="xl"
              borderWidth="1px"
            >
              <form onSubmit={handleUpsertReview}>
                <VStack align="stretch" gap={3}>
                  <Text fontWeight="bold" fontSize="sm">
                    Wystaw / Edytuj opinię
                  </Text>

                  <SimpleGrid
                    columns={{ base: 1, sm: 2 }}
                    gap={4}
                    align="flex-end"
                  >
                    <Field.Root required>
                      <Field.Label>Ocena (gwiazdki)</Field.Label>
                      <NativeSelect.Root>
                        <NativeSelect.Field
                          value={reviewRating}
                          onChange={(e) => setReviewRating(e.target.value)}
                          _dark={{ bg: "rgb(25, 36, 54)" }}
                        >
                          <option value="5">★★★★★ (5)</option>
                          <option value="4">★★★★☆ (4)</option>
                          <option value="3">★★★☆☆ (3)</option>
                          <option value="2">★★☆☆☆ (2)</option>
                          <option value="1">★☆☆☆☆ (1)</option>
                        </NativeSelect.Field>
                      </NativeSelect.Root>
                    </Field.Root>

                    <HStack justify="flex-end" gap={2}>
                      {workshopReviews.some((r) => r.userId === user?.id) && (
                        <Button
                          type="button"
                          size="sm"
                          colorPalette="red"
                          variant="outline"
                          onClick={handleDeleteReview}
                          rounded="lg"
                        >
                          Usuń moją
                        </Button>
                      )}
                      <Button
                        type="submit"
                        size="sm"
                        colorPalette="orange"
                        loading={isSubmittingReview}
                        rounded="lg"
                      >
                        Zapisz
                      </Button>
                    </HStack>
                  </SimpleGrid>

                  <Field.Root>
                    <Field.Label>Komentarz (opcjonalny)</Field.Label>
                    <Textarea
                      placeholder="Napisz opinię o jakości usług, terminowości warsztatu..."
                      value={reviewComment}
                      onChange={(e) => setReviewComment(e.target.value)}
                      rows={2}
                      _dark={{ bg: "rgb(25, 36, 54)" }}
                    />
                  </Field.Root>
                </VStack>
              </form>
            </Box>

            <Separator />

            {/* Reviews List */}
            <VStack align="stretch" gap={4}>
              <Text fontWeight="bold" fontSize="md">
                Wszystkie opinie
              </Text>

              {workshopReviews.length > 0 ? (
                workshopReviews.map((review) => (
                  <Box
                    key={review.id}
                    p={4}
                    bg="white"
                    _dark={{
                      bg: "rgb(15, 23, 42)",
                      borderColor: "whiteAlpha.100",
                    }}
                    rounded="xl"
                    borderWidth="1px"
                    position="relative"
                  >
                    <Flex justify="space-between" align="center" mb={1}>
                      <HStack gap={1} color="orange.400">
                        {[...Array(5)].map((_, i) => (
                          <Icon
                            key={i}
                            as={Star}
                            boxSize={3.5}
                            fill={
                              i < review.rating ? "orange.400" : "transparent"
                            }
                            color="orange.400"
                          />
                        ))}
                        <Text
                          ml={1}
                          fontSize="xs"
                          fontWeight="bold"
                          color="gray.500"
                        >
                          ({review.rating}/5)
                        </Text>
                      </HStack>

                      <Text fontSize="10px" color="gray.400">
                        {new Date(review.createdAt).toLocaleDateString()}
                      </Text>
                    </Flex>
                    {review.comment ? (
                      <Text
                        fontSize="sm"
                        color="gray.700"
                        _dark={{ color: "gray.200" }}
                        mt={1}
                      >
                        {review.comment}
                      </Text>
                    ) : (
                      <Text
                        fontSize="sm"
                        fontStyle="italic"
                        color="gray.400"
                        mt={1}
                      >
                        Brak komentarza tekstowego.
                      </Text>
                    )}

                    {review.userId === user?.id && (
                      <Badge
                        colorPalette="brand"
                        variant="subtle"
                        size="sm"
                        position="absolute"
                        bottom="2"
                        right="3"
                      >
                        Twoja opinia
                      </Badge>
                    )}
                  </Box>
                ))
              ) : (
                <Text fontSize="sm" color="gray.400" textAlign="center" py={6}>
                  Brak opinii o tym warsztacie. Bądź pierwszym, który doda
                  ocenę!
                </Text>
              )}
            </VStack>
          </DialogBody>
          <DialogFooter>
            <DialogActionTrigger asChild>
              <Button variant="ghost" rounded="lg">
                Zamknij
              </Button>
            </DialogActionTrigger>
          </DialogFooter>
          <DialogCloseTrigger />
        </DialogContent>
      </DialogRoot>

      {/* ========================================================
              DIALOG: VEHICLE TIMELINE
              ======================================================== */}
      <DialogRoot
        size="lg"
        open={isTimelineOpen}
        onOpenChange={(e) => setIsTimelineOpen(e.open)}
      >
        <DialogBackdrop />
        <DialogContent _dark={{ bg: "rgb(25, 36, 54)", color: "white" }}>
          <DialogHeader>
            <DialogTitle fontSize="xl" fontWeight="bold">
              Oś czasu pojazdu: {selectedTimelineVehicle?.manufacturer}{" "}
              {selectedTimelineVehicle?.model}
            </DialogTitle>
            <Text fontSize="xs" color="gray.400" mt={1}>
              Rejestracja: {selectedTimelineVehicle?.licensePlate} | VIN:{" "}
              {selectedTimelineVehicle?.vin}
            </Text>
          </DialogHeader>
          <DialogBody maxH="500px" overflowY="auto" py={4}>
            {loadingTimeline ? (
              <VStack gap={4} py={8}>
                <Spinner size="lg" color="brand.500" />
                <Text color="gray.500" fontSize="sm">
                  Pobieranie historii zdarzeń...
                </Text>
              </VStack>
            ) : vehicleTimeline.length > 0 ? (
              <VStack align="stretch" gap={6} position="relative" pl={6}>
                {/* Vertical Timeline Line */}
                <Box
                  position="absolute"
                  left="3.5"
                  top="2"
                  bottom="2"
                  w="2px"
                  bg="gray.200"
                  _dark={{ bg: "whiteAlpha.100" }}
                  zIndex={0}
                />

                {vehicleTimeline.map((item, idx) => {
                  const formatted = formatTimelineEvent(item);
                  return (
                    <Flex
                      key={idx}
                      position="relative"
                      gap={4}
                      align="flex-start"
                      zIndex={1}
                    >
                      {/* Event node dot */}
                      <Flex
                        w={7}
                        h={7}
                        bg={`${formatted.color}.50`}
                        _dark={{ bg: `${formatted.color}.950` }}
                        color={`${formatted.color}.500`}
                        rounded="full"
                        align="center"
                        justify="center"
                        shadow="sm"
                        borderWidth="2px"
                        borderColor={`${formatted.color}.200`}
                        _darkBorder={{ borderColor: `${formatted.color}.800` }}
                        mt={0.5}
                      >
                        <Icon
                          as={
                            formatted.color === "green"
                              ? Check
                              : formatted.color === "red"
                                ? X
                                : formatted.color === "orange"
                                  ? Clock
                                  : formatted.color === "blue"
                                    ? Car
                                    : formatted.color === "yellow"
                                      ? MessageSquare
                                      : formatted.color === "teal"
                                        ? Wrench
                                        : formatted.color === "cyan"
                                          ? FileText
                                          : Calendar
                          }
                          boxSize={3.5}
                        />
                      </Flex>

                      {/* Event Content */}
                      <VStack align="flex-start" gap={0.5} flex={1}>
                        <Flex w="full" justify="space-between" align="baseline">
                          <Text
                            fontWeight="bold"
                            fontSize="sm"
                            color="gray.800"
                            _dark={{ color: "gray.100" }}
                          >
                            {formatted.title}
                          </Text>
                          <Text fontSize="2xs" color="gray.400">
                            {new Date(item.createdAt).toLocaleString("pl-PL", {
                              year: "numeric",
                              month: "2-digit",
                              day: "2-digit",
                              hour: "2-digit",
                              minute: "2-digit",
                            })}
                          </Text>
                        </Flex>
                        <Text
                          fontSize="xs"
                          color="gray.600"
                          _dark={{ color: "gray.300" }}
                        >
                          {formatted.description}
                        </Text>
                      </VStack>
                    </Flex>
                  );
                })}
              </VStack>
            ) : (
              <Center py={12} flexDirection="column">
                <Icon as={Clock} boxSize={12} color="gray.300" mb={4} />
                <Text fontSize="md" fontWeight="bold" color="gray.500">
                  Brak historii zdarzeń
                </Text>
                <Text fontSize="xs" color="gray.400" textAlign="center" mt={1}>
                  Historia i oś czasu pojazdu pojawią się, gdy zaczniesz go
                  edytować lub zlecać naprawy.
                </Text>
              </Center>
            )}
          </DialogBody>
          <DialogFooter>
            <DialogActionTrigger asChild>
              <Button
                type="button"
                variant="outline"
                colorPalette="gray"
                rounded="lg"
              >
                Zamknij
              </Button>
            </DialogActionTrigger>
          </DialogFooter>
          <DialogCloseTrigger />
        </DialogContent>
      </DialogRoot>

      {/* ========================================================
              DIALOG: VEHICLE DOCUMENTS
              ======================================================== */}
      <DialogRoot
        size="lg"
        open={isDocumentsOpen}
        onOpenChange={(e) => setIsDocumentsOpen(e.open)}
      >
        <DialogBackdrop />
        <DialogContent _dark={{ bg: "rgb(25, 36, 54)", color: "white" }}>
          <DialogHeader>
            <DialogTitle fontSize="xl" fontWeight="bold">
              Dokumenty pojazdu: {selectedDocumentsVehicle?.manufacturer}{" "}
              {selectedDocumentsVehicle?.model}
            </DialogTitle>
            <Text fontSize="xs" color="gray.400" mt={1}>
              Rejestracja: {selectedDocumentsVehicle?.licensePlate} | VIN:{" "}
              {selectedDocumentsVehicle?.vin}
            </Text>
          </DialogHeader>
          <DialogBody
            maxH="550px"
            overflowY="auto"
            py={4}
            display="flex"
            flexDirection="column"
            gap={6}
          >
            {/* Upload Document Form */}
            <Box
              p={4}
              bg="gray.50"
              _dark={{ bg: "rgb(15, 23, 42)", borderColor: "whiteAlpha.100" }}
              borderWidth="1px"
              rounded="xl"
              as="form"
              onSubmit={handleUploadDocumentSubmit}
            >
              <Text fontWeight="bold" fontSize="sm" mb={3}>
                Dodaj nowy dokument
              </Text>
              <SimpleGrid columns={{ base: 1, md: 3 }} gap={4} align="flex-end">
                <Field.Root required>
                  <Field.Label fontSize="xs">Typ dokumentu</Field.Label>
                  <NativeSelect.Root size="sm">
                    <NativeSelect.Field
                      value={selectedDocumentType}
                      onChange={(e) => setSelectedDocumentType(e.target.value)}
                      _dark={{ bg: "rgb(25, 36, 54)" }}
                    >
                      <option value="1">Faktura (Invoice)</option>
                      <option value="2">Zdjęcie (Photo)</option>
                      <option value="3">
                        Dowód rejestracyjny (Registration)
                      </option>
                      <option value="4">Inny (Other)</option>
                    </NativeSelect.Field>
                  </NativeSelect.Root>
                </Field.Root>

                <Field.Root required>
                  <Field.Label fontSize="xs">Wybierz plik</Field.Label>
                  <Input
                    type="file"
                    size="sm"
                    onChange={(e) => setSelectedDocumentFile(e.target.files[0])}
                    p={1}
                    height="auto"
                    _dark={{ bg: "rgb(25, 36, 54)" }}
                    rounded="md"
                  />
                </Field.Root>

                <Button
                  type="submit"
                  colorPalette="brand"
                  size="sm"
                  loading={isUploadingDocument}
                  rounded="lg"
                  gap={1.5}
                >
                  <Icon as={Upload} boxSize={3.5} />
                  Prześlij plik
                </Button>
              </SimpleGrid>
            </Box>

            {/* Documents List */}
            <Box>
              <Text fontWeight="bold" fontSize="sm" mb={3}>
                Przesłane dokumenty
              </Text>
              {loadingDocuments ? (
                <VStack gap={4} py={8}>
                  <Spinner size="md" color="brand.500" />
                  <Text color="gray.500" fontSize="xs">
                    Pobieranie dokumentów...
                  </Text>
                </VStack>
              ) : vehicleDocuments.length > 0 ? (
                <VStack align="stretch" gap={3}>
                  {vehicleDocuments.map((doc) => {
                    const getDocTypeLabel = (type) => {
                      switch (type) {
                        case 1:
                          return "Faktura";
                        case 2:
                          return "Zdjęcie";
                        case 3:
                          return "Dowód rejestracyjny";
                        case 4:
                          return "Inny";
                        default:
                          return "Nieznany";
                      }
                    };
                    return (
                      <Flex
                        key={doc.documentId}
                        p={3}
                        bg="white"
                        _dark={{
                          bg: "rgb(15, 23, 42)",
                          borderColor: "whiteAlpha.100",
                        }}
                        borderWidth="1px"
                        rounded="xl"
                        align="center"
                        justify="space-between"
                        gap={4}
                      >
                        <HStack gap={3}>
                          <Flex
                            w={8}
                            h={8}
                            bg="purple.50"
                            _dark={{ bg: "purple.950/20" }}
                            color="purple.500"
                            rounded="lg"
                            align="center"
                            justify="center"
                          >
                            <Icon as={FileText} boxSize={4} />
                          </Flex>
                          <VStack align="flex-start" gap={0}>
                            <Text
                              fontWeight="bold"
                              fontSize="xs"
                              _dark={{ color: "white" }}
                              noOfLines={1}
                              maxW="220px"
                            >
                              {doc.originalFileName || "Dokument"}
                            </Text>
                            <Text fontSize="10px" color="gray.400">
                              Typ: {getDocTypeLabel(doc.documentType)} | Dodano:{" "}
                              {new Date(doc.createdAt).toLocaleDateString()}
                            </Text>
                          </VStack>
                        </HStack>
                        <HStack gap={1.5}>
                          <Button
                            type="button"
                            size="xs"
                            colorPalette="brand"
                            variant="ghost"
                            rounded="md"
                            gap={1}
                            onClick={() =>
                              handleDownloadDocument(
                                selectedDocumentsVehicle.id,
                                doc.documentId,
                                doc.originalFileName,
                              )
                            }
                          >
                            Pobierz
                          </Button>
                          <Button
                            type="button"
                            size="xs"
                            colorPalette="red"
                            variant="ghost"
                            rounded="md"
                            gap={1}
                            onClick={() =>
                              handleDeleteDocument(
                                selectedDocumentsVehicle.id,
                                doc.documentId,
                              )
                            }
                          >
                            Usuń
                          </Button>
                        </HStack>
                      </Flex>
                    );
                  })}
                </VStack>
              ) : (
                <Center
                  py={10}
                  flexDirection="column"
                  borderStyle="dashed"
                  borderWidth="1.5px"
                  borderColor="gray.350"
                  rounded="xl"
                  _dark={{ borderColor: "whiteAlpha.100" }}
                >
                  <Icon as={FileText} boxSize={10} color="gray.300" mb={2} />
                  <Text fontSize="sm" fontWeight="bold" color="gray.500">
                    Brak dokumentów
                  </Text>
                  <Text fontSize="xs" color="gray.400" mt={1}>
                    Ten pojazd nie posiada jeszcze żadnych dokumentów.
                  </Text>
                </Center>
              )}
            </Box>
          </DialogBody>
          <DialogFooter>
            <DialogActionTrigger asChild>
              <Button
                type="button"
                variant="outline"
                colorPalette="gray"
                rounded="lg"
              >
                Zamknij
              </Button>
            </DialogActionTrigger>
          </DialogFooter>
          <DialogCloseTrigger />
        </DialogContent>
      </DialogRoot>
    </Box>
  );
};

export default UserDashboard;
