import { Provider } from "@/components/ui/provider"
import { Box, Button, Heading, Input, InputGroup, SimpleGrid, Text, VStack, Flex, Icon, HStack, Collapsible, CollapsibleTrigger, CollapsibleContent, Field } from "@chakra-ui/react"
import ChakraNavbar from "@/components/layout/ChakraNavbar"
import ChakraHero from "../features/landing/ChakraHero"
import ChakraFeature from "../features/landing/ChakraFeature"
import ChakraFooter from "../components/layout/ChakraFooter"
import { LuSearch } from "react-icons/lu"
import { CircleQuestionMark, FileText, MessageCircle, Search } from "lucide-react"
import { BiQuestionMark } from "react-icons/bi"


const features = [
    {
        title: "Getting started",
        description: "Learn the basics of setting up your account and booking your first service.",
        icon: CircleQuestionMark,
        iconColor: "brand.600",
        iconBgColor: "brand.100",
        darkIconColor: "brand.300",
        darkIconBgColor: "whiteAlpha.200"

    },
    {
        title: "Billing & payments",
        description: "Everything you need to know about payments, invoices and refunds.",
        icon: FileText,
        iconColor: "green.600",
        iconBgColor: "green.100",
        darkIconColor: "green.400",
        darkIconBgColor: "green.700"

    },
    {
        title: "Account support",
        description: "Manage your profile, security settings and notification preferences.",
        icon: MessageCircle,
        iconColor: "purple.500",
        iconBgColor: "purple.200",
        darkIconColor: "purple.400",
        darkIconBgColor: "purple.900"
    }
]

const faq = [
    {
        title: "How do I book a service?",
        description: "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
        icon: CircleQuestionMark
    },
    {
        title: "Can I cancel my appointment?",
        description: "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
        icon: CircleQuestionMark
    },
    {
        title: "How do I contanct the workshop",
        description: "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
        icon: CircleQuestionMark
    }
]

const ChakraHelpPage = () => {
    return (
        <Box minH="100vh" w="full" bg="gray.100" _dark={{ bg: "#0F172A" }}>
            <ChakraNavbar />
            <Box flex="1" p="100px">
                <VStack spacing={8} align="center">

                    <Heading size="4xl" textAlign="center" _dark={{ color: "white" }}>
                        How can we help you?
                    </Heading>

                    <InputGroup w="full" maxW="600px" startElement={<Icon as={Search} />} pt={15} >
                        <Input _focus={{ borderColor: "brand.600", borderWidth: "2px", outline: "none", _dark: { borderColor: "brand.600" } }} rounded="2xl" size="xl" placeholder="Search for articles" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white" }} />
                    </InputGroup>
                    <SimpleGrid columns={3} gap={10} w="full" maxW="1200px" mt={20}>
                        {features.map((features, index) => (
                            <Box w="full" h="full" maxW="400px" display="flex" flexDirection="column" key={index} border="sm" rounded="2xl" p="6" boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }} transition="all 0.3 ease" _hover={{ cursor: "pointer", boxShadow: "2xl" }}>
                                <Flex align="center" justify="center" w={11} h={11} bg={features.iconBgColor} rounded="xl" mb={6} color={features.iconColor} _dark={{ bg: features.darkIconBgColor, color: features.darkIconColor }}>
                                    <Icon boxSize={6} strokeWidth={2} as={features.icon} />
                                </Flex>
                                <Heading size="xl">{features.title}</Heading>
                                <Text color="gray.600" pt={3} _dark={{ color: "gray.400" }}>{features.description}</Text>
                            </Box>
                        ))}
                    </SimpleGrid>
                </VStack>
                <VStack mt={20} align="flex-start" w="full" maxW="1200px" mx="auto">
                    <Heading size="2xl" _dark={{ color: "white" }}>Frequently Asked Questions</Heading>
                    <SimpleGrid w="full" gap={4} maxW="800px" mx="auto" columns="1" mt={10}>
                        {faq.map((faq, index) => (
                            <Collapsible.Root key={index}>
                                <Box w="full" justifyContent="flex-start" displayflexDirection="column" key={index} border="sm" rounded="2xl" boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }} transition="all 0.3 ease" _hover={{ borderColor: "brand.600", cursor: "pointer" }}>
                                    <Collapsible.Trigger rounded="2xl" display="flex" textAlign="left" p="6" w="full" fontWeight="semibold" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }} _hover={{ cursor: "pointer" }}>{faq.title}</Collapsible.Trigger>
                                    <Collapsible.Content>
                                        <Text color="gray.600" pt={3} _dark={{ color: "gray.400" }} p="6">{faq.description}</Text>
                                    </Collapsible.Content>
                                </Box>
                            </Collapsible.Root>
                        ))}
                    </SimpleGrid>
                </VStack>
            </Box>
            <ChakraFooter />
        </Box>

    )
}

export default ChakraHelpPage;