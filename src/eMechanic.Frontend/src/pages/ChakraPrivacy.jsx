import { Box, Button, Heading, InputGroup, SimpleGrid, Text, VStack, Flex, Icon, HStack, Collapsible, CollapsibleTrigger, CollapsibleContent, Field, Fieldset, For, Input, NativeSelect, Textarea, List } from "@chakra-ui/react"
import ChakraNavbar from "@/components/layout/ChakraNavbar"
import ChakraHero from "../features/landing/ChakraHero"
import ChakraFeature from "../features/landing/ChakraFeature"
import ChakraFooter from "../components/layout/ChakraFooter"

const privacyData = [
    {
        title: "Information we collect",
        description: "We collect information you provide directly to us when you create an account, update your profile, or communicate with us. This may include:",
        subItems: [
            "Name and contact information",
            "Vehicle information",
            "Service history and preferences",
            "Payment information"
        ]
    },
    {
        title: "How We Use Your Information",
        description: "We use the information we collect to:",
        subItems: [
            "Provide, maintain, and improve our services",
            "Process transactions and send related information",
            "Send you technical notices, updates, and support messages",
            "Connect you with workshops and facilitate appointments",
        ]
    },
    {
        title: "Information Sharing",
        description: "We share your information with workshops when you book a service. We do not sell your personal information to third parties. We may share generic aggregated demographic information not linked to any personal identification information."
    },
    {
        title: "Data Security",
        description: "We implement appropriate data collection, storage, and processing practices and security measures to protect against unauthorized access, alteration, disclosure, or destruction of your personal information."
    },
    {
        title: "Your Rights",
        description: "You have the right to access, correct, or delete your personal information. You can manage your information through your account settings or by contacting us."
    },
    {
        title: "Contact Us",
        description: "If you have any questions about this Privacy Policy, please contact us at privacy@emechanic.com."
    },
]

const ChakraPrivacy = () => {
    return (
        <Box minH="100vh" w="full" bg="gray.100" _dark={{ bg: "#0F172A" }} display={"flex"} flexDirection={"column"}>
            <ChakraNavbar />
            <Box flex="1" p={90} maxW="1400px" mx="auto" mt={10}>
                <VStack align={"flex-start"} w="full" h="full" maxW="900px" display="flex" flexDirection="column" border="sm" rounded="2xl" p="6" boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }}>
                    <Heading size={"3xl"} fontWeight={"bold"}>Privacy Policy</Heading>
                    <Text py={5}>Last updated: 27.02.2026</Text>
                    <List.Root as={"ol"} px={10} gap={3}>
                        {privacyData.map((item, index) => (
                            <List.Item key={index} fontWeight="semibold" fontSize="xl"  _marker={{ color: "brand.600" }}>
                                {item.title}

                                <List.Root ps={5} listStyleType="none" gap={3} mt={3} fontWeight="normal" fontSize="md">


                                    <List.Item color="gray.600" _dark={{ color: "gray.400" }} >
                                        {item.description}

                                        {item.subItems && (
                                            <List.Root as="ul" ps={10} listStyle="disc" mt={3}>
                                                {item.subItems.map((subItem, subIndex) => (
                                                    <List.Item key={subIndex}  _marker={{ color: "brand.600" }}>
                                                        {subItem}
                                                    </List.Item>
                                                ))}
                                            </List.Root>
                                        )}

                                    </List.Item>

                                </List.Root>
                            </List.Item>
                        ))}
                    </List.Root>
                </VStack>
            </Box>
            <ChakraFooter />
        </Box>
    );
}
export default ChakraPrivacy; 