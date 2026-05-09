import { Box, Button, Heading, InputGroup, SimpleGrid, Text, VStack, Flex, Icon, HStack, Collapsible, CollapsibleTrigger, CollapsibleContent, Field, Fieldset, For, Input, NativeSelect, Textarea, } from "@chakra-ui/react"
import ChakraNavbar from "@/components/layout/ChakraNavbar"
import ChakraHero from "../features/landing/ChakraHero"
import ChakraFeature from "../features/landing/ChakraFeature"
import ChakraFooter from "../components/layout/ChakraFooter"

const ChakraAbout = () => {
    return (
        <Box minH="100vh" w="full" bg="gray.100" _dark={{ bg: "#0F172A" }} display={"flex"} flexDirection={"column"}>
            <ChakraNavbar />
            <Box flex="1" p={90} maxW="1400px" mx="auto" mt={10}>
                <VStack align="flex-start">
                    <Heading size={"4xl"} fontWeight={"bold"} _dark={{ color: "white" }}>About Us</Heading>
                    <Text fontSize={"xl"} my={7} _dark={{ color: "whiteAlpha.700" }}>Welcome to eMechanic, your trusted partner in automotive care. We are dedicated to connecting car owners with the best, most reliable workshops in their area.</Text>
                    <Text fontSize={"xl"} my={7} _dark={{ color: "whiteAlpha.700" }}>Our mission is to bring transparency, efficiency, and trust to the car repair industry. Whether you need a routine oil change or a complex engine repair, eMechanic makes it easy to find, compare, and book services.</Text>
                    <Heading size={"4xl"} fontWeight={"bold"} _dark={{ color: "white" }}>Our Story</Heading>
                    <Text fontSize={"xl"} my={7} _dark={{ color: "whiteAlpha.700" }}>Founded in 2025, eMechanic started with a simple idea: car repair shouldn't be a hassle. We saw a need for a platform that empowers both car owners and workshop owners, creating a seamless experience for everyone involved.</Text>
                </VStack>
            </Box>
            <ChakraFooter />
        </Box>
    );
}
export default ChakraAbout; 