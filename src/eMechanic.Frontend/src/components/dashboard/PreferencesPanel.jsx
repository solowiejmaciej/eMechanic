import React from "react";
import {
  Box,
  Flex,
  Button,
  Heading,
  SimpleGrid,
  VStack,
  Text,
  Separator,
  Spinner,
  Center,
} from "@chakra-ui/react";

export const PreferencesPanel = ({
  parts,
  setParts,
  timeline,
  setTimeline,
  loading,
  submitting,
  onSubmit,
}) => {
  if (loading) {
    return (
      <VStack gap={6} align="stretch">
        <Heading size="2xl" fontWeight="black" tracking="tight">Repair preferences</Heading>
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
          My repair preferences
        </Heading>
        <Text color="gray.500" _dark={{ color: "gray.400" }} fontSize="md" mt={1}>
          Select your default preferences for replacement parts and repair turnaround times. Repair shops will receive this information when they provide a quote.
        </Text>
      </Box>

      <Box
        p={6}
        bg="white"
        _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
        borderWidth="1px"
        borderColor="gray.200"
        rounded="2xl"
        boxShadow="0 15px 35px -5px rgba(59, 130, 246, 0.06)"
        as="form"
        onSubmit={onSubmit}
      >
        <VStack align="stretch" gap={8}>
          <Box>
            <Text fontSize="lg" fontWeight="bold" mb={3} _dark={{ color: "white" }}>
              Preferred Replacement Parts
            </Text>
            <SimpleGrid columns={{ base: 1, md: 3 }} gap={4}>
              {[
                {
                  value: "1",
                  title: "Economic",
                  desc: "Lowest price, certified replacement parts.",
                },
                {
                  value: "2",
                  title: "Balanced",
                  desc: "Good value for the price; recommended alternatives.",
                },
                {
                  value: "3",
                  title: "Premium",
                  desc: "Original Equipment Manufacturer (OEM) parts or the highest quality.",
                },
              ].map((item) => (
                <Box
                  key={item.value}
                  p={5}
                  borderWidth="2.5px"
                  borderRadius="xl"
                  cursor="pointer"
                  borderColor={parts === item.value ? "brand.500" : "gray.200"}
                  bg={parts === item.value ? "brand.50/10" : "transparent"}
                  _dark={{
                    borderColor: parts === item.value ? "brand.500" : "whiteAlpha.150",
                    bg: parts === item.value ? "brand.900/10" : "transparent",
                  }}
                  onClick={() => setParts(item.value)}
                  transition="all 0.2s cubic-bezier(0.4, 0, 0.2, 1)"
                  _hover={{
                    borderColor: "brand.500",
                    transform: "translateY(-2px)",
                    boxShadow: "sm",
                  }}
                >
                  <Text
                    fontWeight="extrabold"
                    fontSize="md"
                    color={parts === item.value ? "brand.600" : "gray.700"}
                    _dark={{
                      color: parts === item.value ? "brand.400" : "white",
                    }}
                  >
                    {item.title}
                  </Text>
                  <Text mt={1.5} fontSize="xs" color="gray.500" _dark={{ color: "gray.400" }}>
                    {item.desc}
                  </Text>
                </Box>
              ))}
            </SimpleGrid>
          </Box>

          <Box>
            <Text fontSize="lg" fontWeight="bold" mb={3} _dark={{ color: "white" }}>
              Turnaround time for repair
            </Text>
            <SimpleGrid columns={{ base: 1, md: 2 }} gap={4}>
              {[
                {
                  value: "1",
                  title: "Standard",
                  desc: "Standard turnaround time for service work.",
                },
                {
                  value: "2",
                  title: "Urgent",
                  desc: "Express repair turnaround time (may incur an additional fee).",
                },
              ].map((item) => (
                <Box
                  key={item.value}
                  p={5}
                  borderWidth="2.5px"
                  borderRadius="xl"
                  cursor="pointer"
                  borderColor={timeline === item.value ? "brand.500" : "gray.200"}
                  bg={timeline === item.value ? "brand.50/10" : "transparent"}
                  _dark={{
                    borderColor: timeline === item.value ? "brand.500" : "whiteAlpha.150",
                    bg: timeline === item.value ? "brand.900/10" : "transparent",
                  }}
                  onClick={() => setTimeline(item.value)}
                  transition="all 0.2s cubic-bezier(0.4, 0, 0.2, 1)"
                  _hover={{
                    borderColor: "brand.500",
                    transform: "translateY(-2px)",
                    boxShadow: "sm",
                  }}
                >
                  <Text
                    fontWeight="extrabold"
                    fontSize="md"
                    color={timeline === item.value ? "brand.600" : "gray.700"}
                    _dark={{
                      color: timeline === item.value ? "brand.400" : "white",
                    }}
                  >
                    {item.title}
                  </Text>
                  <Text mt={1.5} fontSize="xs" color="gray.500" _dark={{ color: "gray.400" }}>
                    {item.desc}
                  </Text>
                </Box>
              ))}
            </SimpleGrid>
          </Box>

          <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} />

          <Flex justify="flex-end">
            <Button
              type="submit"
              colorPalette="brand"
              loading={submitting}
              rounded="xl"
              px={8}
              fontWeight="bold"
              shadow="md"
              _hover={{ transform: "translateY(-1px)", shadow: "lg" }}
            >
              Save preferences
            </Button>
          </Flex>
        </VStack>
      </Box>
    </VStack>
  );
};

export default PreferencesPanel;
