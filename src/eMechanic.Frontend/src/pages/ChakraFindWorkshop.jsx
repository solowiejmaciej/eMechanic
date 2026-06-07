import { Box, Button, Heading, InputGroup, SimpleGrid, Text, Flex, Icon, HStack, Input, Skeleton, Badge, Separator, Image, VStack } from "@chakra-ui/react"
import ChakraNavbar from "@/components/layout/ChakraNavbar"
import ChakraHero from "../features/landing/ChakraHero"
import ChakraFeature from "../features/landing/ChakraFeature"
import ChakraFooter from "../components/layout/ChakraFooter"
import { Search, MapPin, Star, Phone, Mail, Wrench } from 'lucide-react';
import { useState, useEffect } from 'react'
import { getWorkshops, getWorkshopDocuments } from '../api/workshops';

const WorkshopCard = ({ workshop }) => {
  const [image, setImage] = useState(null);

  useEffect(() => {
    const fetchImage = async () => {
      try {
        const docs = await getWorkshopDocuments(workshop.id, { pageNumber: 1, pageSize: 50 });
        const logo = docs?.items?.find(d => d.type === 1 || d.type === "Logo" || d.type === "logo");
        const gallery = docs?.items?.find(d => d.type === 2 || d.type === "GalleryImage" || d.type === "galleryImage");
        if (logo?.publicUrl) setImage(logo.publicUrl);
        else if (gallery?.publicUrl) setImage(gallery.publicUrl);
      } catch (e) {
        console.error("Failed to fetch documents for workshop", workshop.id, e);
      }
    };
    fetchImage();
  }, [workshop.id]);

  return (
    <Box 
      p={6} 
      bg="white" 
      _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }} 
      rounded="2xl" 
      borderWidth="1px" 
      borderColor="gray.200"
      w="full" 
      shadow="md"
      display="flex"
      flexDirection="column"
      gap={4}
      transition="all 0.2s"
      _hover={{ transform: "translateY(-4px)", shadow: "lg" }}
    >
      <Flex gap={4} align="center">
        {image ? (
          <Image 
            src={image} 
            alt={workshop.displayName} 
            boxSize="60px" 
            objectFit="cover" 
            rounded="xl" 
            fallback={<Flex boxSize="60px" bg="orange.100" _dark={{ bg: "orange.900/30" }} align="center" justify="center" rounded="xl"><Icon as={Wrench} color="orange.500" boxSize={6} /></Flex>}
          />
        ) : (
          <Flex boxSize="60px" bg="orange.50" _dark={{ bg: "orange.900/20" }} align="center" justify="center" rounded="xl">
            <Icon as={Wrench} color="orange.500" boxSize={6} />
          </Flex>
        )}
        <VStack align="flex-start" gap={1}>
          <Heading size="md" fontWeight="bold" _dark={{ color: "white" }}>
            {workshop.displayName || "Nazwa warsztatu"}
          </Heading>
          <HStack gap={1}>
            <Icon as={MapPin} color="gray.400" boxSize={3.5} />
            <Text fontSize="xs" color="gray.500" _dark={{ color: "gray.400" }}>
              {workshop.city || "Brak miasta"}
            </Text>
          </HStack>
        </VStack>
      </Flex>

      <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />

      <VStack align="stretch" gap={2} fontSize="sm">
        <HStack gap={2}>
          <Icon as={MapPin} color="brand.500" boxSize={4} />
          <Text color="gray.600" _dark={{ color: "gray.300" }} noOfLines={1}>
            {workshop.address ? `${workshop.address}, ${workshop.postalCode} ${workshop.city}` : "Brak adresu"}
          </Text>
        </HStack>

        {workshop.phoneNumber && (
          <HStack gap={2}>
            <Icon as={Phone} color="brand.500" boxSize={4} />
            <Text color="gray.600" _dark={{ color: "gray.300" }}>
              {workshop.phoneNumber}
            </Text>
          </HStack>
        )}

        {workshop.contactEmail && (
          <HStack gap={2}>
            <Icon as={Mail} color="brand.500" boxSize={4} />
            <Text color="gray.600" _dark={{ color: "gray.300" }} noOfLines={1}>
              {workshop.contactEmail}
            </Text>
          </HStack>
        )}
      </VStack>

      <Flex justify="space-between" align="center" mt={2}>
        <Badge colorPalette="orange" variant="subtle" rounded="md" px={2} py={0.5}>
          Warsztat
        </Badge>
        <Button size="sm" colorPalette="brand" variant="outline" rounded="lg">
          Szczegóły
        </Button>
      </Flex>
    </Box>
  );
}

const WorkshopCardSkeleton = () => (
  <Box 
    p={6} 
    bg="white" 
    _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }} 
    rounded="2xl" 
    borderWidth="1px" 
    borderColor="gray.200"
    w="full" 
    shadow="md"
    display="flex"
    flexDirection="column"
    gap={4}
  >
    <Flex gap={4} align="center">
      <Skeleton boxSize="60px" rounded="xl" />
      <VStack align="flex-start" gap={2} flex={1}>
        <Skeleton h="20px" w="70%" />
        <Skeleton h="14px" w="40%" />
      </VStack>
    </Flex>
    <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />
    <VStack align="stretch" gap={2}>
      <Skeleton h="16px" w="90%" />
      <Skeleton h="16px" w="60%" />
      <Skeleton h="16px" w="80%" />
    </VStack>
    <Flex justify="space-between" align="center" mt={2}>
      <Skeleton h="24px" w="80px" rounded="md" />
      <Skeleton h="32px" w="100px" rounded="lg" />
    </Flex>
  </Box>
);

const ChakraFindWorshop = () => {
  const [workshops, setWorkshops] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchPhrase, setSearchPhrase] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const pageSize = 10;
  const [totalItems, setTotalItems] = useState(0);

  const fetchWorkshops = async () => {
    setLoading(true);
  
    try {
      const params = {
        PageNumber: pageNumber,
        PageSize: pageSize,
        SearchPhrase: searchPhrase || undefined
      };
      const data = await getWorkshops(params);
      setWorkshops(data.items || []);
      setTotalPages(data.totalPages || 1);
      setTotalItems(data.totalCount || 0);
    } catch (e) {
      console.error("Failed to fetch workshops", e);
    } finally {
      setLoading(false);

    }
  };

  useEffect(() => {
    fetchWorkshops();
  }, [pageNumber]);

  const handleSearch = (e) => {
    e.preventDefault();
    if (pageNumber === 1) {
      fetchWorkshops();
    } else {
      setPageNumber(1);
    }
  };

  const handlePageChange = (newPage) => {
    if (newPage >= 1 && newPage <= totalPages) {
      setPageNumber(newPage);
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  };

  const startItem = (pageNumber - 1) * pageSize + 1;
  const endItem = Math.min(pageNumber * pageSize, totalItems);

  return (
    <Box minH="100vh" w="full" bg="gray.100" _dark={{ bg: "#0F172A" }} display={"flex"} flexDirection={"column"}>
      <ChakraNavbar />
      <Box flex="1" p={{ base: 5, md: 10, lg: 90 }} mx="auto" mt={10} maxW={"1400px"} w="full">
        
        <Heading size={"3xl"} fontWeight={"bold"} _dark={{ color: "white" }}>Find a Workshop</Heading>
        
        <HStack gap={"5"} w={"full"} pt={15}>
          <InputGroup w="full" maxW="1400px" startElement={<Icon as={Search} />}>
            <Input 
              _focus={{ borderColor: "brand.600", borderWidth: "2px", outline: "none", _dark: { borderColor: "brand.600" } }} 
              w="full" 
              rounded="2xl" 
              size="xl" 
              placeholder="Search for workshops" 
              _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white" }} 
              value={searchPhrase} 
              onChange={(e) => setSearchPhrase(e.target.value)}
            />
          </InputGroup>
          <Button bg="brand.600" size={"xl"} rounded="md" _hover={{ bg: "brand.700" }} _dark={{ color: "white" }} onClick={handleSearch}>
            Search
          </Button>
        </HStack>

        <Flex 
          mt={10} 
          minH="250px" 
          direction="column" 
          justify={(!loading && workshops.length > 0) ? "flex-start" : "center"}
          align={(!loading && workshops.length > 0) ? "stretch" : "center"}
          w="full"
        >
         {loading ? ( 
              <Flex direction="column" gap={6} w="full">
                <SimpleGrid columns={{ base: 1, md: 2, lg: 3 }} gap={6} w="full">
                  {[...Array(6)].map((_, i) => (
                    <WorkshopCardSkeleton key={i} />
                  ))}
                </SimpleGrid>
              </Flex>
          ) : workshops.length > 0 ? (
              <Flex direction="column" gap={6} w="full">
                  
                  <Flex justify="space-between" align="center" w="full" flexDir={{ base: "column", sm: "row" }} gap={4}>
                      <Text color="gray.600" _dark={{ color: "gray.400" }}>
                          Showing <Text as="span" fontWeight="bold" color="brand.600">{startItem}-{endItem}</Text> of <Text as="span" fontWeight="bold" color="brand.600">{totalItems}</Text> results
                      </Text>
                      
                      <HStack gap={2}>
                          <Button variant="outline" colorPalette="brand" disabled={pageNumber === 1} onClick={() => handlePageChange(pageNumber - 1)}>
                            Previous
                          </Button>
                          <Text fontWeight="bold" px={4} _dark={{ color: "white" }}>
                            {pageNumber} / {totalPages}
                          </Text>
                          <Button variant="outline" colorPalette="brand" disabled={pageNumber === totalPages} onClick={() => handlePageChange(pageNumber + 1)}>
                            Next
                          </Button>
                      </HStack>
                  </Flex>

                  <SimpleGrid columns={{ base: 1, md: 2, lg: 3 }} gap={6} w="full">
                    {workshops.map((workshop) => (
                      <WorkshopCard key={workshop.id} workshop={workshop} />
                    ))}
                  </SimpleGrid>

              </Flex>
          ) : (
              <Text fontSize="xl" color="gray.500" textAlign="center">
                  No workshops found.
              </Text>
          )}
        </Flex>

      </Box>
      <ChakraFooter />
    </Box >
  );
};

export default ChakraFindWorshop;