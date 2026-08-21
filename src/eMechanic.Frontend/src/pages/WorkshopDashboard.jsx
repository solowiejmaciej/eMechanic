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
import {
  getWorkshopRequests,
  provideEstimation,
  getById as getRepairRequestById,
} from "../api/repairRequests";
import {
  getWorkshopRepairs,
  startRepair,
  completeRepair,
} from "../api/repairs";
import {
  getReviews,
  getReviewStats,
  uploadWorkshopDocument,
  deleteWorkshopDocument,
  getWorkshopDocuments,
  downloadWorkshopDocument,
} from "../api/workshops";
import { getById as getVehicleById } from "../api/vehicles";
import { getRepairPreferencesForWorkshop } from "../api/user";

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
    description:
      "Stukanie w przednim zawieszeniu przy skręcaniu oraz na nierównościach.",
    diagnosis: "",
    estimatedCostAmount: null,
    estimatedCostCurrency: "PLN",
    clientName: "Jan Kowalski",
    clientEmail: "jan.kowalski@gmail.com",
    vehicle: {
      manufacturer: "Skoda",
      model: "Octavia",
      licensePlate: "KR 12345",
      vin: "TMBGG7NE8G210492",
    },
  },
  {
    id: "req-2",
    status: 2, // Wycenione
    createdAt: "2026-06-04T12:30:00Z",
    description:
      "Wyciek oleju spod silnika, zapaliła się czerwona kontrolka ciśnienia.",
    diagnosis:
      "Uszkodzona uszczelka pokrywy zaworów. Konieczna wymiana uszczelki oraz mycie silnika.",
    estimatedCostAmount: 1200,
    estimatedCostCurrency: "PLN",
    clientName: "Anna Nowak",
    clientEmail: "anna.nowak@onet.pl",
    vehicle: {
      manufacturer: "BMW",
      model: "Seria 3",
      licensePlate: "WI 99999",
      vin: "WBA8E1C5XGF20194",
    },
  },
  {
    id: "req-3",
    status: 3, // Zaakceptowane
    createdAt: "2026-06-03T15:45:00Z",
    description: "Klimatyzacja słabo chłodzi, słychać głośny szum z nawiewów.",
    diagnosis:
      "Nieszczelność chłodnicy klimatyzacji (skraplacza). Konieczna wymiana chłodnicy oraz napełnienie czynnika.",
    estimatedCostAmount: 850,
    estimatedCostCurrency: "PLN",
    clientName: "Piotr Wiśniewski",
    clientEmail: "piotr.wisniewski@wp.pl",
    vehicle: {
      manufacturer: "Ford",
      model: "Focus",
      licensePlate: "GD 7777A",
      vin: "WF0FXXWGCF8D1049",
    },
  },
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
    vehicle: {
      manufacturer: "Volkswagen",
      model: "Golf",
      licensePlate: "WA 44455",
      vin: "WVWZZZ1KZD810294",
    },
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
    vehicle: {
      manufacturer: "Audi",
      model: "A4",
      licensePlate: "PO 88888",
      vin: "WAUZZZ8K9CA20492",
    },
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
    vehicle: {
      manufacturer: "Opel",
      model: "Astra",
      licensePlate: "DW 33322",
      vin: "W0L0AHL358G29482",
    },
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
    vehicle: {
      manufacturer: "Toyota",
      model: "Corolla",
      licensePlate: "GD 11223",
      vin: "JTDKN32E0010492",
    },
  },
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
  },
];

const initialReviews = [
  {
    id: "rev-1",
    rating: 5,
    comment:
      "Świetny kontakt, szybka diagnoza usterki zawieszenia i ekspresowa naprawa. Cena zgodna z wyceną. Polecam!",
    clientName: "Jan Kowalski",
    createdAt: "2026-06-06T15:20:00Z",
  },
  {
    id: "rev-2",
    rating: 5,
    comment:
      "Wymiana rozrządu w Audi wykonana profesjonalnie. Dostałem zdjęcia starego paska i części. Bardzo uczciwy warsztat.",
    clientName: "Małgorzata Wójcik",
    createdAt: "2026-06-05T18:40:00Z",
  },
  {
    id: "rev-3",
    rating: 4,
    comment:
      "Klimatyzacja w końcu działa poprawnie, choć usługa trwała kilka godzin dłużej niż planowano. Mimo to polecam za fachowość.",
    clientName: "Piotr Wiśniewski",
    createdAt: "2026-06-04T12:15:00Z",
  },
];

const initialStats = {
  averageRating: 4.8,
  totalReviews: 3,
  distribution: { 5: 2, 4: 1, 3: 0, 2: 0, 1: 0 },
};

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

const WorkshopDashboard = ({ activeMenu, setActiveMenu }) => {
  const { user } = useAuth();

  // States holding data from API
  const [repairRequests, setRepairRequests] = useState([]);
  const [repairs, setRepairs] = useState([]);
  const [documents, setDocuments] = useState([]);
  const [reviews, setReviews] = useState([]);
  const [stats, setStats] = useState(initialStats);

  // Requests Pagination/Search State
  const [requestsPage, setRequestsPage] = useState(1);
  const [requestsPageSize, setRequestsPageSize] = useState(5);
  const [requestsSearchPhrase, setRequestsSearchPhrase] = useState("");
  const [requestsStatus, setRequestsStatus] = useState("All");
  const [requestsTotalPages, setRequestsTotalPages] = useState(1);
  const [requestsTotalCount, setRequestsTotalCount] = useState(0);

  // Repairs Pagination/Search State
  const [repairsPage, setRepairsPage] = useState(1);
  const [repairsPageSize, setRepairsPageSize] = useState(5);
  const [repairsSearchPhrase, setRepairsSearchPhrase] = useState("");
  const [repairsStatus, setRepairsStatus] = useState("All");
  const [repairsTotalPages, setRepairsTotalPages] = useState(1);
  const [repairsTotalCount, setRepairsTotalCount] = useState(0);

  // Reviews Pagination/Search State
  const [reviewsPage, setReviewsPage] = useState(1);
  const [reviewsPageSize, setReviewsPageSize] = useState(5);
  const [reviewsSearchPhrase, setReviewsSearchPhrase] = useState("");
  const [reviewsRating, setReviewsRating] = useState("All");
  const [reviewsTotalPages, setReviewsTotalPages] = useState(1);
  const [reviewsTotalCount, setReviewsTotalCount] = useState(0);

  const [userPreferences, setUserPreferences] = useState({});
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
  const fetchRequests = async (
    page = requestsPage,
    size = requestsPageSize,
    search = requestsSearchPhrase,
    status = requestsStatus,
  ) => {
    try {
      const searchPhraseParam =
        search.trim() || (status !== "All" ? status : null);
      const reqData = await getWorkshopRequests({
        PageNumber: page,
        PageSize: size,
        SearchPhrase: searchPhraseParam,
      });
      const items = reqData.items || [];

      const itemsWithVehicles = await Promise.all(
        items.map(async (item) => {
          try {
            if (item.vehicleId) {
              const vehicle = await getVehicleById(item.vehicleId);
              return {
                ...item,
                vehicle,
                clientName:
                  vehicle.clientFirstName && vehicle.clientLastName
                    ? `${vehicle.clientFirstName} ${vehicle.clientLastName}`
                    : vehicle.clientEmail || "Client data missing",
                clientEmail: vehicle.clientEmail || "E-mail missing",
              };
            }
          } catch (vehicleErr) {
            console.error(
              `Failed to fetch vehicle for ID ${item.vehicleId}:`,
              vehicleErr,
            );
          }
          return {
            ...item,
            vehicle: null,
            clientName: "Client data missing",
            clientEmail: "E-mail missing",
          };
        }),
      );

      setRepairRequests(itemsWithVehicles);
      setRequestsPage(reqData.pageNumber || page);
      setRequestsTotalPages(reqData.totalPages || 1);
      setRequestsTotalCount(reqData.totalCount || items.length);

      // Proactively fetch client preferences for the requests
      const uniqueUserIds = new Set();
      itemsWithVehicles.forEach((item) => {
        if (item.vehicle?.userId) uniqueUserIds.add(item.vehicle.userId);
      });
      if (uniqueUserIds.size > 0) {
        const prefsMap = { ...userPreferences };
        await Promise.all(
          Array.from(uniqueUserIds).map(async (uid) => {
            if (!prefsMap[uid]) {
              try {
                const prefs = await getRepairPreferencesForWorkshop(uid);
                prefsMap[uid] = prefs;
              } catch (err) {
                console.error(
                  `Failed to fetch repair preferences for user ${uid}:`,
                  err,
                );
              }
            }
          }),
        );
        setUserPreferences(prefsMap);
      }

      return itemsWithVehicles;
    } catch (err) {
      console.error(
        "Failed to fetch requests from API, falling back to mock data:",
        err,
      );
      let filtered = initialRequests;
      if (search.trim()) {
        const phrase = search.toLowerCase();
        filtered = filtered.filter(
          (x) =>
            x.description.toLowerCase().includes(phrase) ||
            (x.diagnosis && x.diagnosis.toLowerCase().includes(phrase)),
        );
      } else if (status !== "All") {
        filtered = filtered.filter((x) => {
          const s = getRequestStatusNumber(x.status);
          switch (status) {
            case "Pending":
              return s === 1;
            case "Estimated":
              return s === 2;
            case "Accepted":
              return s === 3;
            case "Rejected":
              return s === 4;
            default:
              return true;
          }
        });
      }
      const count = filtered.length;
      const pages = Math.ceil(count / size) || 1;
      const paginatedItems = filtered.slice((page - 1) * size, page * size);

      setRepairRequests(paginatedItems);
      setRequestsPage(page);
      setRequestsTotalPages(pages);
      setRequestsTotalCount(count);
      return paginatedItems;
    }
  };

  const fetchRepairs = async (
    page = repairsPage,
    size = repairsPageSize,
    search = repairsSearchPhrase,
    status = repairsStatus,
  ) => {
    try {
      const searchPhraseParam =
        search.trim() || (status !== "All" ? status : null);
      const repData = await getWorkshopRepairs({
        PageNumber: page,
        PageSize: size,
        SearchPhrase: searchPhraseParam,
      });
      const items = repData.items || [];

      const itemsWithVehicles = await Promise.all(
        items.map(async (item) => {
          let requestData = null;
          if (item.repairRequestId) {
            try {
              requestData = await getRepairRequestById(item.repairRequestId);
            } catch (reqErr) {
              console.error(
                `Failed to fetch repair request for ID ${item.repairRequestId}:`,
                reqErr,
              );
            }
          }

          try {
            if (item.vehicleId) {
              console.log(requestData);
              const vehicle = await getVehicleById(item.vehicleId);
              return {
                ...item,
                vehicle,
                description: requestData?.description || "Description missing",
                diagnosis: requestData?.diagnosis || "Diagnose missing",
                clientName:
                  vehicle.clientFirstName && vehicle.clientLastName
                    ? `${vehicle.clientFirstName} ${vehicle.clientLastName}`
                    : vehicle.clientEmail || "Client data missing",
                clientEmail: vehicle.clientEmail || "E-mail missing",
              };
            }
          } catch (vehicleErr) {
            console.error(
              `Failed to fetch vehicle for ID ${item.vehicleId}:`,
              vehicleErr,
            );
          }
          return {
            ...item,
            vehicle: null,
            description: requestData?.description || "Description missing",
            diagnosis: requestData?.diagnosis || "Diagnose missing",
            clientName: "Client data missing",
            clientEmail: "E-mail missing",
          };
        }),
      );

      setRepairs(itemsWithVehicles);
      setRepairsPage(repData.pageNumber || page);
      setRepairsTotalPages(repData.totalPages || 1);
      setRepairsTotalCount(repData.totalCount || items.length);

      // Proactively fetch client preferences for the repairs
      const uniqueUserIds = new Set();
      itemsWithVehicles.forEach((item) => {
        if (item.vehicle?.userId) uniqueUserIds.add(item.vehicle.userId);
      });
      if (uniqueUserIds.size > 0) {
        const prefsMap = { ...userPreferences };
        await Promise.all(
          Array.from(uniqueUserIds).map(async (uid) => {
            if (!prefsMap[uid]) {
              try {
                const prefs = await getRepairPreferencesForWorkshop(uid);
                prefsMap[uid] = prefs;
              } catch (err) {
                console.error(
                  `Failed to fetch repair preferences for user ${uid}:`,
                  err,
                );
              }
            }
          }),
        );
        setUserPreferences(prefsMap);
      }

      return itemsWithVehicles;
    } catch (err) {
      console.error(
        "Failed to fetch repairs from API, falling back to mock data:",
        err,
      );
      let filtered = initialRepairs;
      if (search.trim()) {
        const phrase = search.toLowerCase();
        filtered = filtered.filter(
          (x) =>
            x.description.toLowerCase().includes(phrase) ||
            (x.diagnosis && x.diagnosis.toLowerCase().includes(phrase)),
        );
      } else if (status !== "All") {
        filtered = filtered.filter((x) => {
          const s = getStatusNumber(x.status);
          switch (status) {
            case "Scheduled":
              return s === 0;
            case "InProgress":
              return s === 1;
            case "Completed":
              return s === 2;
            case "Paid":
              return s === 3;
            default:
              return true;
          }
        });
      }
      const count = filtered.length;
      const pages = Math.ceil(count / size) || 1;
      const paginatedItems = filtered.slice((page - 1) * size, page * size);

      setRepairs(paginatedItems);
      setRepairsPage(page);
      setRepairsTotalPages(pages);
      setRepairsTotalCount(count);
      return paginatedItems;
    }
  };

  const fetchDocs = async () => {
    if (!user?.id) return false;
    try {
      const docsData = await getWorkshopDocuments(user.id, {
        PageNumber: 1,
        PageSize: 50,
      });
      setDocuments(docsData.items || []);
      return true;
    } catch (err) {
      console.error("Failed to fetch documents from API:", err);
      toaster.create({
        title: "Download error",
        description: formatErrorMsg(
          err,
          "Failed to fetch documents from API.",
        ),
        type: "error",
      });
      return false;
    }
  };

  const fetchReviewsData = async (
    page = reviewsPage,
    size = reviewsPageSize,
    search = reviewsSearchPhrase,
    rating = reviewsRating,
  ) => {
    if (!user?.id) {
      let filtered = initialReviews;
      if (search.trim()) {
        const phrase = search.toLowerCase();
        filtered = filtered.filter(
          (x) => x.comment && x.comment.toLowerCase().includes(phrase),
        );
      } else if (rating !== "All") {
        filtered = filtered.filter((x) => x.rating === parseInt(rating, 10));
      }
      const count = filtered.length;
      const pages = Math.ceil(count / size) || 1;
      const paginatedItems = filtered.slice((page - 1) * size, page * size);
      setReviews(paginatedItems);
      setReviewsPage(page);
      setReviewsTotalPages(pages);
      setReviewsTotalCount(count);
      return true;
    }
    try {
      const searchPhraseParam =
        search.trim() || (rating !== "All" ? rating : null);
      const revs = await getReviews(user.id, {
        PageNumber: page,
        PageSize: size,
        SearchPhrase: searchPhraseParam,
      });
      setReviews(revs.items || []);
      setReviewsPage(revs.pageNumber || page);
      setReviewsTotalPages(revs.totalPages || 1);
      setReviewsTotalCount(revs.totalCount || (revs.items || []).length);

      const rStats = await getReviewStats(user.id);
      setStats(rStats || initialStats);
      return true;
    } catch (err) {
      console.error(
        "Failed to fetch reviews from API, falling back to mock data:",
        err,
      );
      let filtered = initialReviews;
      if (search.trim()) {
        const phrase = search.toLowerCase();
        filtered = filtered.filter(
          (x) => x.comment && x.comment.toLowerCase().includes(phrase),
        );
      } else if (rating !== "All") {
        filtered = filtered.filter((x) => x.rating === parseInt(rating, 10));
      }
      const count = filtered.length;
      const pages = Math.ceil(count / size) || 1;
      const paginatedItems = filtered.slice((page - 1) * size, page * size);

      setReviews(paginatedItems);
      setReviewsPage(page);
      setReviewsTotalPages(pages);
      setReviewsTotalCount(count);
      setStats(initialStats);
      return false;
    }
  };

  // --- REQUESTS EVENT HANDLERS ---
  const handleRequestsPageChange = (page) => {
    setRequestsPage(page);
    fetchRequests(page, requestsPageSize, requestsSearchPhrase, requestsStatus);
  };

  const handleRequestsPageSizeChange = (size) => {
    setRequestsPageSize(size);
    setRequestsPage(1);
    fetchRequests(1, size, requestsSearchPhrase, requestsStatus);
  };

  const handleRequestsSearchChange = (phrase) => {
    setRequestsSearchPhrase(phrase);
    setRequestsPage(1);
    if (phrase.trim()) {
      setRequestsStatus("All");
      fetchRequests(1, requestsPageSize, phrase, "All");
    } else {
      fetchRequests(1, requestsPageSize, "", requestsStatus);
    }
  };

  const handleRequestsStatusChange = (status) => {
    setRequestsStatus(status);
    setRequestsPage(1);
    if (status !== "All") {
      setRequestsSearchPhrase("");
      fetchRequests(1, requestsPageSize, "", status);
    } else {
      fetchRequests(1, requestsPageSize, requestsSearchPhrase, "All");
    }
  };

  // --- REPAIRS EVENT HANDLERS ---
  const handleRepairsPageChange = (page) => {
    setRepairsPage(page);
    fetchRepairs(page, repairsPageSize, repairsSearchPhrase, repairsStatus);
  };

  const handleRepairsPageSizeChange = (size) => {
    setRepairsPageSize(size);
    setRepairsPage(1);
    fetchRepairs(1, size, repairsSearchPhrase, repairsStatus);
  };

  const handleRepairsSearchChange = (phrase) => {
    setRepairsSearchPhrase(phrase);
    setRepairsPage(1);
    if (phrase.trim()) {
      setRepairsStatus("All");
      fetchRepairs(1, repairsPageSize, phrase, "All");
    } else {
      fetchRepairs(1, repairsPageSize, "", repairsStatus);
    }
  };

  const handleRepairsStatusChange = (status) => {
    setRepairsStatus(status);
    setRepairsPage(1);
    if (status !== "All") {
      setRepairsSearchPhrase("");
      fetchRepairs(1, repairsPageSize, "", status);
    } else {
      fetchRepairs(1, repairsPageSize, repairsSearchPhrase, "All");
    }
  };

  // --- REVIEWS EVENT HANDLERS ---
  const handleReviewsPageChange = (page) => {
    setReviewsPage(page);
    fetchReviewsData(page, reviewsPageSize, reviewsSearchPhrase, reviewsRating);
  };

  const handleReviewsPageSizeChange = (size) => {
    setReviewsPageSize(size);
    setReviewsPage(1);
    fetchReviewsData(1, size, reviewsSearchPhrase, reviewsRating);
  };

  const handleReviewsSearchChange = (phrase) => {
    setReviewsSearchPhrase(phrase);
    setReviewsPage(1);
    if (phrase.trim()) {
      setReviewsRating("All");
      fetchReviewsData(1, reviewsPageSize, phrase, "All");
    } else {
      fetchReviewsData(1, reviewsPageSize, "", reviewsRating);
    }
  };

  const handleReviewsRatingChange = (rating) => {
    setReviewsRating(rating);
    setReviewsPage(1);
    if (rating !== "All") {
      setReviewsSearchPhrase("");
      fetchReviewsData(1, reviewsPageSize, "", rating);
    } else {
      fetchReviewsData(1, reviewsPageSize, reviewsSearchPhrase, "All");
    }
  };

  // Fetch all initial data
  useEffect(() => {
    const loadDashboardData = async () => {
      setLoading(true);

      const reqs = await fetchRequests(1, 5, "", "All");
      const reps = await fetchRepairs(1, 5, "", "All");

      const reqsOk = reqs !== null;
      const repsOk = reps !== null;

      let revsOk = true;
      let docsOk = true;
      if (user?.id) {
        revsOk = await fetchReviewsData(1, 5, "", "All");
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
        title: "Cost estimation sent",
        description: "The quote was successfully sent and saved in the API.",
        type: "success",
      });
    } catch (err) {
      console.error("Failed to submit estimation to API:", err);
      toaster.create({
        title: "Pricing Error",
        description: formatErrorMsg(err, "The quote could not be saved to the API."),
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
        title: "Repair began",
        description: "The repair status has been updated on the server.",
        type: "success",
      });
    } catch (err) {
      console.error("Failed to start repair on API:", err);
      toaster.create({
        title: "Repair Start Error",
        description: formatErrorMsg(
          err,
          "The repair status could not be updated via the API.",
        ),
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
        title: "Repair completed",
        description: "The final cost has been saved on the server.",
        type: "success",
      });
    } catch (err) {
      console.error("Failed to complete repair on API:", err);
      toaster.create({
        title: "Repair Completion Error",
        description: formatErrorMsg(
          err,
          "Failed to save the final cost in the API.",
        ),
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
          title: "Document added",
          description: `Document "${mockDoc.displayName}" was successfully sent to the server.`,
          type: "success",
        });
      } catch (err) {
        console.error("Failed to upload document to API:", err);
        toaster.create({
          title: "Document upload error",
          description: formatErrorMsg(
            err,
            "Failed to upload document to API.",
          ),
          type: "error",
        });
      }
    } else {
      toaster.create({
        title: "Error",
        description: "You must select a file to upload.",
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
          title: "Document deleted",
          description: "The document has been deleted from the server.",
          type: "success",
        });
      } catch (err) {
        console.error("Failed to delete document from API:", err);
        toaster.create({
          title: "Document Delete Error",
          description: formatErrorMsg(
            err,
            "Failed to delete document from API.",
          ),
          type: "error",
        });
      }
    }
  };

  // Action: Download/Preview Workshop Document via Blob
  const handleDownloadWorkshopDocument = async (docId, fileName) => {
    if (!user?.id) return;
    try {
      const blob = await downloadWorkshopDocument(user.id, docId);
      const url = window.URL.createObjectURL(new Blob([blob]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", fileName);
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
    } catch (err) {
      console.error("Failed to download workshop document:", err);
      toaster.create({
        title: "Download Error",
        description: "Failed to download workshop document.",
        type: "error",
      });
    }
  };

  // Action: Save profile details
  const handleProfileSubmit = (e) => {
    e.preventDefault();
    setProfileSubmitting(true);
    setTimeout(() => {
      setProfileSubmitting(false);
      toaster.create({
        title: "Saved profile data",
        description:
          "The information about your workshop has been successfully updated.",
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
  const activeRepairsCount = repairs.filter(
    (r) => getStatusNumber(r.status) === 1,
  ).length;
  const vehiclesInShopCount = repairs.filter((r) => {
    const s = getStatusNumber(r.status);
    return s === 0 || s === 1 || s === 2;
  }).length;
  const newRequestsCount = repairRequests.filter(
    (r) => getRequestStatusNumber(r.status) === 1,
  ).length;

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
          {/* Dashboard Logo or Header */}
          <Box>
            <Text
              fontSize="10px"
              fontWeight="bold"
              color="orange.500"
              tracking="widest"
              textTransform="uppercase"
            >
              Mechanic Panel
            </Text>
            <Heading
              size="md"
              fontWeight="bold"
              mt={0.5}
              _dark={{ color: "white" }}
            >
              {profileDisplayName || "My workshop"}
            </Heading>
            {isOfflineMode && (
              <Badge colorPalette="orange" variant="outline" mt={1}>
                Offline mode
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
                bg:
                  activeMenu === "requests" ? "whiteAlpha.100" : "transparent",
              }}
              gap={3}
            >
              <Icon as={Clock} boxSize={5} />
              Requests
              {newRequestsCount > 0 && (
                <Badge
                  colorPalette="orange"
                  variant="solid"
                  rounded="full"
                  ml="auto"
                  px={2}
                >
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
              Active Repairs
              {activeRepairsCount > 0 && (
                <Badge
                  colorPalette="blue"
                  variant="solid"
                  rounded="full"
                  ml="auto"
                  px={2}
                >
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
                bg:
                  activeMenu === "documents" ? "whiteAlpha.100" : "transparent",
              }}
              gap={3}
            >
              <Icon as={FolderOpen} boxSize={5} />
              Documents
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
              Opinions
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
              Profile
            </Button>
          </VStack>
        </VStack>
      </Box>

      {/* --- RIGHT CONTENT AREA --- */}
      <Box
        flex="1"
        p={8}
        overflowY="auto"
        h="calc(100vh - 80px)"
        display="flex"
        flexDirection="column"
        gap={8}
      >
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
                <Text
                  fontSize="xs"
                  fontWeight="bold"
                  color="gray.400"
                  textTransform="uppercase"
                >
                  Active repairs
                </Text>
                <Text
                  fontSize="3xl"
                  fontWeight="black"
                  color="gray.800"
                  _dark={{ color: "white" }}
                >
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
                <Text
                  fontSize="xs"
                  fontWeight="bold"
                  color="gray.400"
                  textTransform="uppercase"
                >
                  Vehicles in the shop
                </Text>
                <Text
                  fontSize="3xl"
                  fontWeight="black"
                  color="gray.800"
                  _dark={{ color: "white" }}
                >
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
                <Text
                  fontSize="xs"
                  fontWeight="bold"
                  color="gray.400"
                  textTransform="uppercase"
                >
                  New requests
                </Text>
                <Text
                  fontSize="3xl"
                  fontWeight="black"
                  color="gray.800"
                  _dark={{ color: "white" }}
                >
                  {newRequestsCount}
                </Text>
              </VStack>
            </Flex>
          </Box>
        </SimpleGrid>

        <Separator
          borderColor="gray.200"
          _dark={{ borderColor: "whiteAlpha.100" }}
        />

        {/* --- MAIN ACTIVE PANEL CONTAINER --- */}
        <Box flex="1">
          {activeMenu === "requests" && (
            <WorkshopRequestsPanel
              repairRequests={repairRequests}
              loading={loading}
              onProvideEstimation={handleProvideEstimation}
              userPreferences={userPreferences}
              pageNumber={requestsPage}
              totalPages={requestsTotalPages}
              pageSize={requestsPageSize}
              onPageChange={handleRequestsPageChange}
              onPageSizeChange={handleRequestsPageSizeChange}
              searchPhrase={requestsSearchPhrase}
              onSearchChange={handleRequestsSearchChange}
              statusFilter={requestsStatus}
              onStatusFilterChange={handleRequestsStatusChange}
            />
          )}

          {activeMenu === "repairs" && (
            <WorkshopRepairsPanel
              repairs={repairs}
              loading={loading}
              onStartRepair={handleStartRepair}
              onCompleteRepair={handleCompleteRepair}
              userPreferences={userPreferences}
              pageNumber={repairsPage}
              totalPages={repairsTotalPages}
              pageSize={repairsPageSize}
              onPageChange={handleRepairsPageChange}
              onPageSizeChange={handleRepairsPageSizeChange}
              searchPhrase={repairsSearchPhrase}
              onSearchChange={handleRepairsSearchChange}
              statusFilter={repairsStatus}
              onStatusFilterChange={handleRepairsStatusChange}
            />
          )}

          {activeMenu === "documents" && (
            <WorkshopDocumentsPanel
              documents={documents}
              loading={loading}
              onUploadDocument={handleUploadDocument}
              onDeleteDocument={handleDeleteDocument}
              onDownloadDocument={handleDownloadWorkshopDocument}
            />
          )}

          {activeMenu === "reviews" && (
            <WorkshopReviewsPanel
              reviews={reviews}
              stats={stats}
              loading={loading}
              pageNumber={reviewsPage}
              totalPages={reviewsTotalPages}
              pageSize={reviewsPageSize}
              onPageChange={handleReviewsPageChange}
              onPageSizeChange={handleReviewsPageSizeChange}
              searchPhrase={reviewsSearchPhrase}
              onSearchChange={handleReviewsSearchChange}
              ratingFilter={reviewsRating}
              onRatingFilterChange={handleReviewsRatingChange}
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
