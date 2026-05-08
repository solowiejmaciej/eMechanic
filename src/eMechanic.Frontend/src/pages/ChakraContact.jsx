import { Provider } from "@/components/ui/provider"
import { Box, Button, Heading, InputGroup, SimpleGrid, Text, VStack, Flex, Icon, HStack, Collapsible, CollapsibleTrigger, CollapsibleContent, Field, Fieldset, For, Input, NativeSelect, Textarea, } from "@chakra-ui/react"
import ChakraNavbar from "@/components/layout/ChakraNavbar"
import ChakraHero from "../features/landing/ChakraHero"
import ChakraFeature from "../features/landing/ChakraFeature"
import ChakraFooter from "../components/layout/ChakraFooter"
import { LuSearch } from "react-icons/lu"
import { CircleQuestionMark, FileText, Mail, MapPin, MessageCircle, Phone, Pin, Search, TicketX } from "lucide-react"
import { BiQuestionMark } from "react-icons/bi"


const ChakraContact = () => {
    return (
        <Box minH="100vh" w="full" bg="gray.100" _dark={{ bg: "#0F172A" }}>
            <ChakraNavbar />
            <Box flex="1" p={100} maxW="1400px" mx="auto" mt={20}>
                <Heading size="3xl" fontWeight="bold" _dark={{ color: "white" }}>Contact Us</Heading>
                <SimpleGrid columns={{ base: "1", md: "2" }} gap={20} mb={200} mt={10} alignItems={"start"}>
                    <VStack align="flex-start" gap={0}>
                        <Text fontSize="lg" _dark={{ color: "whiteAlpha.800" }}>Have questions or need assistance? We're here to help! Reach out to our support team using the contact information below or fill out the form.</Text>
                        <HStack mt={10} align="flex-start" gap={5}>
                            <Flex align="center" justify="center" minW={12} w={12} h={12} bg="brand.50" rounded="2xl" mb={6} color="brand.600" _dark={{ bg: "blue.900", color: "brand.300" }}>
                                <Icon boxSize={6} strokeWidth={1.5} as={Mail} />
                            </Flex>
                            <VStack align="flex-start">
                                <Heading size='lg' _dark={{ color: "white" }}>Email Us</Heading>
                                <Text _dark={{ color: "whiteAlpha.700" }}> support@emechanic.com</Text>
                                <Text _dark={{ color: "whiteAlpha.700" }} lineHeight={0.5}> partners@emechanic.com</Text>
                            </VStack>
                        </HStack>
                        <HStack mt={10} align="flex-start" gap={5}>
                            <Flex align="center" justify="center" minW={12} w={12} h={12} bg="brand.50" rounded="2xl" mb={6} color="brand.600" _dark={{ bg: "blue.900", color: "brand.300" }}>
                                <Icon boxSize={6} strokeWidth={1.5} as={Phone} />
                            </Flex>
                            <VStack align="flex-start">
                                <Heading size='lg' _dark={{ color: "white" }}>Email Us</Heading>
                                <Text _dark={{ color: "whiteAlpha.700" }}> +1 (555) 123-4567 </Text>
                                <Text _dark={{ color: "whiteAlpha.700" }} lineHeight={0.5}> Mon-Fri, 9am - 5pm EST </Text>
                            </VStack>
                        </HStack>
                        <HStack mt={10} align="flex-start" gap={5}>
                            <Flex align="center" justify="center" minW={12} w={12} h={12} bg="brand.50" rounded="2xl" mb={6} color="brand.600" _dark={{ bg: "blue.900", color: "brand.300" }}>
                                <Icon boxSize={6} strokeWidth={1.5} as={MapPin} />
                            </Flex>
                            <VStack align="flex-start">
                                <Heading size='lg' _dark={{ color: "white" }}>Visit Us</Heading>
                                <Text _dark={{ color: "whiteAlpha.700" }}> 123 Auto Lane</Text>
                                <Text _dark={{ color: "whiteAlpha.700" }} lineHeight={0.5}>Mechanic City, MC 12345</Text>
                            </VStack>
                        </HStack>
                    </VStack>
                    <VStack bg="white" rounded="2xl" shadow="lg" p={10} border="1px solid" borderColor="gray.100" _dark={{ bg: "rgb(25, 36, 54)", borderColor: "rgba(30, 41, 59, 0.5)" }}>
                        <Fieldset.Root alignContent={"center"} size={"lg"}>
                            <Fieldset.Legend _dark={{ color: "white" }} fontSize="xl" fontWeight="bold">Send us a message</Fieldset.Legend>
                            <Field.Root>
                                <Field.Label _dark={{ color: "white" }}>Name</Field.Label>
                                <Input placeholder="Your name" bg="gray.100" _focus={{ borderColor: "brand.600", outline: "none" }} _dark={{ bg: "#0F172A", color: "white" }} rounded={"xl"} fontSize={18} />
                            </Field.Root>
                            <Field.Root>
                                <Field.Label _dark={{ color: "white" }}>Email</Field.Label>
                                <Input placeholder="your@email.com" bg="gray.100" _focus={{ borderColor: "brand.600", outline: "none" }} _dark={{ bg: "#0F172A", color: "white" }} rounded={"xl"} fontSize={18} />
                            </Field.Root>
                            <Field.Root>
                                <Field.Label _dark={{ color: "white" }}>Message</Field.Label>
                                <Textarea name="message" bg="gray.100" placeholder="How can we help?" _focus={{ borderColor: "brand.600", outline: "none" }} _dark={{ bg: "#0F172A", color: "white" }} rows={4} rounded={"xl"} fontSize={18} />
                            </Field.Root>
                            <Button type="submit" _hover={{ cursor: "pointer", bg: "brand.500" }} color={"white"} bg={"brand.600"} rounded={"2xl"} size={"xl"}>
                                Send a message
                            </Button>
                        </Fieldset.Root>
                    </VStack>
                </SimpleGrid>
            </Box>
            <ChakraFooter />
        </Box>

    )
}

export default ChakraContact;