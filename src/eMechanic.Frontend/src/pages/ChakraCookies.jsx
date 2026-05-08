import { Provider } from "@/components/ui/provider"
import { Box, Button, Heading, InputGroup, SimpleGrid, Text, VStack, Flex, Icon, HStack, Collapsible, CollapsibleTrigger, CollapsibleContent, Field, Fieldset, For, Input, NativeSelect, Textarea, } from "@chakra-ui/react"
import ChakraNavbar from "@/components/layout/ChakraNavbar"
import ChakraHero from "../features/landing/ChakraHero"
import ChakraFeature from "../features/landing/ChakraFeature"
import ChakraFooter from "../components/layout/ChakraFooter"
import { LuSearch } from "react-icons/lu"
import { CircleQuestionMark, FileText, Mail, MapPin, MessageCircle, Phone, Pin, Search, TicketX } from "lucide-react"
import { BiQuestionMark } from "react-icons/bi"

const ChakraCookies = () => {
    return (
        <Box minH="100vh" w="full" bg="gray.100" _dark={{ bg: "#0F172A" }} display={"flex"} flexDirection={"column"}>
            <ChakraNavbar />
            <Box flex="1" p={90} maxW="1400px" mx="auto" mt={10}>
                <VStack align="flex-start">
                    <Heading size={"4xl"} fontWeight={"bold"} _dark={{ color: "white" }}>Cookie Policy</Heading>
                    <Text fontSize={"xl"} my={7} _dark={{ color: "whiteAlpha.700" }}>This Cookie Policy explains how eMechanic uses cookies and similar technologies to recognize you when you visit our website.</Text>
                    <Heading size={"4xl"} fontWeight={"bold"} _dark={{ color: "white" }}>What are cookies?</Heading>
                    <Text fontSize={"xl"} my={7} _dark={{ color: "whiteAlpha.700" }}>Cookies are small data files that are placed on your computer or mobile device when you visit a website. Cookies are widely used by website owners in order to make their websites work, or to work more efficiently, as well as to provide reporting information.</Text>
                    <Heading size={"4xl"} fontWeight={"bold"} _dark={{ color: "white" }}>How we use cookies</Heading>
                    <Text fontSize={"xl"} my={7} _dark={{ color: "whiteAlpha.700" }}>We use cookies for several reasons. Some cookies are required for technical reasons in order for our website to operate, and we refer to these as "essential" or "strictly necessary" cookies. Other cookies also enable us to track and target the interests of our users to enhance the experience on our Online Properties.</Text>
                </VStack>
            </Box>
            <ChakraFooter />
        </Box>
    );
}
export default ChakraCookies; 