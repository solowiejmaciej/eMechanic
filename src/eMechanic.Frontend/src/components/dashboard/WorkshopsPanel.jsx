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
  Center,
  InputGroup,
  Input,
  Separator,
  Skeleton,
} from "@chakra-ui/react";
import {
  Search,
  Wrench,
  MapPin,
  Phone,
  Mail,
  MessageSquare,
  Plus,
} from "lucide-react";

export const WorkshopsPanel = ({
  workshops,
  loadingWorkshops,
  searchWorkshop,
  setSearchWorkshop,
  handleWorkshopSearch,
  workshopTotalCount,
  workshopPage,
  workshopTotalPages,
  setWorkshopPage,
  openReviewsModal,
  openRepairRequestModal,
}) => {
  return (
    <VStack align="stretch" gap={6}>
      <Box>
        <Heading size="2xl" fontWeight="black" tracking="tight" _dark={{ color: "white" }}>
          Serach Workshop
        </Heading>
        <Text color="gray.500" _dark={{ color: "gray.400" }} fontSize="md" mt={1}>
          Choose a workshop in your area, write a review, or report a problem directly
        </Text>
      </Box>

      <HStack gap={4} pt={2}>
        <InputGroup w="full" startElement={<Icon as={Search} />}>
          <Input
            _focus={{
              borderColor: "brand.500",
              borderWidth: "2px",
              outline: "none",
            }}
            w="full"
            rounded="xl"
            size="lg"
            placeholder="Wpisz nazwę warsztatu lub miasto..."
            _dark={{ bg: "rgb(25, 36, 54)", color: "white" }}
            value={searchWorkshop}
            onChange={(e) => setSearchWorkshop(e.target.value)}
          />
        </InputGroup>
        <Button
          bg="brand.500"
          size="lg"
          rounded="xl"
          fontWeight="bold"
          _hover={{ bg: "brand.600" }}
          _dark={{ color: "white" }}
          onClick={handleWorkshopSearch}
          shadow="md"
        >
          Szukaj
        </Button>
      </HStack>

      {loadingWorkshops ? (
        <SimpleGrid columns={{ base: 1, md: 2, lg: 3 }} gap={6} w="full">
          {[...Array(6)].map((_, i) => (
            <Box
              key={i}
              p={6}
              bg="white"
              _dark={{ bg: "rgb(25, 36, 54)" }}
              rounded="2xl"
              borderWidth="1px"
              borderColor="gray.200"
              gap={4}
              display="flex"
              flexDirection="column"
            >
              <Flex gap={4} align="center">
                <Skeleton boxSize="60px" rounded="xl" />
                <VStack align="flex-start" gap={2} flex={1}>
                  <Skeleton h="20px" w="70%" />
                  <Skeleton h="14px" w="40%" />
                </VStack>
              </Flex>
              <Separator />
              <VStack align="stretch" gap={2}>
                <Skeleton h="16px" w="90%" />
                <Skeleton h="16px" w="60%" />
              </VStack>
              <Flex justify="space-between" mt={2}>
                <Skeleton h="24px" w="80px" />
                <Skeleton h="32px" w="100px" />
              </Flex>
            </Box>
          ))}
        </SimpleGrid>
      ) : workshops.length > 0 ? (
        <VStack align="stretch" gap={6}>
          <Flex
            justify="space-between"
            align="center"
            flexDir={{ base: "column", sm: "row" }}
            gap={4}
          >
            <Text color="gray.600" _dark={{ color: "gray.400" }} fontSize="sm" fontWeight="medium">
              Found{" "}
              <Text as="span" fontWeight="bold" color="brand.500">
                {workshopTotalCount}
              </Text>{" "}
              workshops
            </Text>

            <HStack gap={2}>
              <Button
                variant="outline"
                size="sm"
                rounded="lg"
                disabled={workshopPage === 1}
                onClick={() => setWorkshopPage((p) => Math.max(p - 1, 1))}
              >
                Back
              </Button>
              <Text fontSize="sm" fontWeight="bold">
                {workshopPage} / {workshopTotalPages}
              </Text>
              <Button
                variant="outline"
                size="sm"
                rounded="lg"
                disabled={workshopPage === workshopTotalPages}
                onClick={() =>
                  setWorkshopPage((p) =>
                    Math.min(p + 1, workshopTotalPages)
                  )
                }
              >
                Next
              </Button>
            </HStack>
          </Flex>

          <SimpleGrid columns={{ base: 1, md: 2, lg: 3 }} gap={6} w="full">
            {workshops.map((workshop) => (
              <Box
                key={workshop.id}
                p={6}
                bg="white"
                _dark={{
                  bg: "rgb(25, 36, 54)",
                  borderColor: "whiteAlpha.100",
                }}
                rounded="2xl"
                borderWidth="1px"
                borderColor="gray.200"
                w="full"
                boxShadow="0 10px 25px -5px rgba(59, 130, 246, 0.05), 0 8px 10px -6px rgba(59, 130, 246, 0.05)"
                display="flex"
                flexDirection="column"
                gap={4}
                transition="all 0.3s cubic-bezier(0.4, 0, 0.2, 1)"
                _hover={{
                  transform: "translateY(-4px)",
                  boxShadow: "0 20px 30px -10px rgba(59, 130, 246, 0.12), 0 10px 15px -3px rgba(59, 130, 246, 0.08)",
                  borderColor: "brand.300",
                }}
              >
                <Flex gap={4} align="center">
                  <Flex
                    boxSize="60px"
                    bg="brand.50"
                    _dark={{ bg: "brand.900/20" }}
                    align="center"
                    justify="center"
                    rounded="xl"
                  >
                    <Icon as={Wrench} color="brand.500" boxSize={6} />
                  </Flex>
                  <VStack align="flex-start" gap={0}>
                    <Heading size="md" fontWeight="bold" _dark={{ color: "white" }}>
                      {workshop.displayName}
                    </Heading>
                    <HStack gap={1}>
                      <Icon as={MapPin} color="gray.400" boxSize={3.5} />
                      <Text fontSize="xs" color="gray.500" _dark={{ color: "gray.400" }}>
                        {workshop.city}
                      </Text>
                    </HStack>
                  </VStack>
                </Flex>

                <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />

                <VStack align="stretch" gap={2} fontSize="sm">
                  <HStack gap={2}>
                    <Icon as={MapPin} color="brand.500" boxSize={4} />
                    <Text color="gray.600" _dark={{ color: "gray.300" }} noOfLines={1}>
                      {workshop.address}, {workshop.postalCode} {workshop.city}
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

                <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />

                <Flex justify="space-between" align="center" gap={2} mt="auto">
                  <Button
                    size="sm"
                    colorPalette="brand"
                    variant="ghost"
                    rounded="lg"
                    onClick={() => openReviewsModal(workshop)}
                    gap={1.5}
                    _hover={{ bg: "brand.50", _dark: { bg: "brand.900/20" } }}
                  >
                    <Icon as={MessageSquare} boxSize={4} />
                    Opinions
                  </Button>
                  <Button
                    size="sm"
                    colorPalette="brand"
                    variant="solid"
                    rounded="lg"
                    onClick={() => openRepairRequestModal(workshop)}
                    gap={1.5}
                    fontWeight="semibold"
                    shadow="sm"
                  >
                    <Icon as={Plus} boxSize={4} />
                    Schedule a repair
                  </Button>
                </Flex>
              </Box>
            ))}
          </SimpleGrid>
        </VStack>
      ) : (
        <Center py={16} flexDirection="column" borderWidth="1.5px" borderStyle="dashed" borderColor="gray.350" rounded="2xl" _dark={{ borderColor: "whiteAlpha.100" }}>
          <Icon as={Wrench} boxSize={16} color="gray.300" mb={4} />
          <Text fontSize="lg" fontWeight="bold" color="gray.500">
            No workshops
          </Text>
          <Text fontSize="sm" color="gray.400" textAlign="center" mt={1}>
            Try changing the keywords in the search field.
          </Text>
        </Center>
      )}
    </VStack>
  );
};

export default WorkshopsPanel;
