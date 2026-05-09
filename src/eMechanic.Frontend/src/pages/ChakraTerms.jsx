import { Provider } from "@/components/ui/provider"
import { Box, List, Button, Heading, InputGroup, SimpleGrid, Text, VStack, Flex, Icon, HStack, Collapsible, CollapsibleTrigger, CollapsibleContent, Field, Fieldset, For, Input, NativeSelect, Textarea, Link } from "@chakra-ui/react"
import ChakraNavbar from "@/components/layout/ChakraNavbar"
import ChakraHero from "../features/landing/ChakraHero"
import ChakraFeature from "../features/landing/ChakraFeature"
import ChakraFooter from "../components/layout/ChakraFooter"
import { LuSearch } from "react-icons/lu"
import { BarChart3, Calendar, CircleCheck, CircleCheckBigIcon, CircleQuestionMark, CloudLightning, FileText, Mail, MapPin, MessageCircle, Phone, Pin, Search, TicketX, Users, Wrench, Zap } from "lucide-react"
import { BiQuestionMark } from "react-icons/bi"
import React from "react"

const termsData = [
    {
        title: "Acceptance of Terms",
        description: "By accessing and using eMechanic, you agree to be bound by these Terms of Service. If you do not agree to these terms, please do not use the Service.",
    },
    {
        title: "User Accounts",
        description: (
        <>
            When you create an account with us, you must provide information that is accurate, complete, and current at all times. Failure to do so constitutes a breach of the Terms, which may result in immediate termination of your account on our Service.
            <br /><br />
            You are responsible for safeguarding the password that you use to access the Service and for any activities or actions under your password.
        </>
    ),
    },
    {
        title: "Services",
        description: "eMechanic connects vehicle owners with automotive workshops. We are not responsible for the quality of services provided by workshops. Any agreement or transaction is strictly between the vehicle owner and the workshop. "
    },
    {
        title: "Intellectual Property",
        description: "The Service and its original content, features, and functionality are and will remain the exclusive property of eMechanic and its licensors."
    },
    {
        title: "Termination",
        description: "We may terminate or suspend access to our Service immediately, without prior notice or liability, for any reason whatsoever, including without limitation if you breach the Terms."
    },
    {
        title: "Contact Us",
        description: "If you have any questions about this Privacy Policy, please contact us at privacy@emechanic.com."
    },
]

const ChakraTerms = () => {
    return (
        <Box minH="100vh" w="full" bg="gray.100" _dark={{ bg: "#0F172A" }} display={"flex"} flexDirection={"column"}>
            <ChakraNavbar />
            <Box flex="1" p={90} maxW="900px" mx="auto" mt={10}>
                <VStack align={"flex-start"} w="full" h="full" maxW="900px" display="flex" flexDirection="column" border="sm" rounded="2xl" p="6" boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }}>
                    <Heading size={"3xl"} fontWeight={"bold"}>Terms of Service</Heading>
                    <Text py={5}>Last updated: 10.04.2026</Text>
                    <List.Root as={"ol"} px={10} gap={3}>
                        {termsData.map((item, index) => (
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
    )
}

export default ChakraTerms;