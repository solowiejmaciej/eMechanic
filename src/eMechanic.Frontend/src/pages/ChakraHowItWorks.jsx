import { Provider } from "@/components/ui/provider"
import { Box, Button, Heading, InputGroup, SimpleGrid, Text, VStack, Flex, Icon, HStack, Collapsible, CollapsibleTrigger, CollapsibleContent, Field, Fieldset, For, Input, NativeSelect, Textarea, } from "@chakra-ui/react"
import ChakraNavbar from "@/components/layout/ChakraNavbar"
import ChakraHero from "../features/landing/ChakraHero"
import ChakraFeature from "../features/landing/ChakraFeature"
import ChakraFooter from "../components/layout/ChakraFooter"
import { LuSearch } from "react-icons/lu"
import { Calendar, CircleCheck, CircleCheckBigIcon, CircleQuestionMark, FileText, Mail, MapPin, MessageCircle, Phone, Pin, Search, TicketX, Wrench } from "lucide-react"
import { BiQuestionMark } from "react-icons/bi"


const ChakraHowItWorks = () => {
    return (
        <Box display={"flex"} minH="100vh" w="full" bg="gray.100" _dark={{ bg: "#0F172A" }} flexDirection={"column"}>
            <ChakraNavbar />
            <Box flex="1" p={100} maxW="1400px" mx="auto">
                <VStack textAlign={"center"}>
                <Heading size="5xl" textAlign="center" fontWeight={"bold"} _dark={{ color: "white" }}>How eMechanic Works</Heading>
                <Text maxW={"600px"} fontSize={"lg"} color="gray.600" pt={3} _dark={{ color: "gray.400" }}>Getting your car repaired shouldn't be complicated. We've simplified the process into 4 easy steps.</Text>
                </VStack>
                <SimpleGrid columns={{base: 1, lg: 4}} mt={10}>
                    <VStack>
                        <Flex position="relative" align={"center"} justify={"center"} w={16} h={16} mb={2} bg={"brand.600/30"} rounded="full" color={"brand.200"} _dark={{ bg: "brand.700/30", color: "brand.700"}}>
                             <Icon strokeWidth={2}boxSize={8} as={Search} />
                             <Flex top={-2} right={-2} position={"absolute"} align={"center"} justify={"center"} w={8} h={8} rounded={"full"} bg={"orange.500"}> 
                                <Text fontWeight={"bold"} color={"white"}>1</Text>
                             </Flex>
                         </Flex>
                         <Heading color="gray.600" _dark={{ color: "white" }}>Search</Heading>
                         <Text textAlign={"center"} maxW={"275px"} color="gray.600" _dark={{ color: "gray.400" }}>Enter your location and the service you need. Browse top-rated local workshops.</Text>
                    </VStack>
                    <VStack>
                        <Flex position="relative" align={"center"} justify={"center"} w={16} h={16} mb={2} bg={"brand.600/30"} rounded="full" color={"brand.200"} _dark={{ bg: "brand.700/30", color: "brand.700"}}>
                             <Icon strokeWidth={2}boxSize={8} as={Calendar} />
                             <Flex top={-2} right={-2} position={"absolute"} align={"center"} justify={"center"} w={8} h={8} rounded={"full"} bg={"orange.500"}> 
                                <Text fontWeight={"bold"} color={"white"}>2</Text>
                             </Flex>
                         </Flex>
                         <Heading color="gray.600" _dark={{ color: "white" }}>Book</Heading>
                         <Text textAlign={"center"} maxW={"250px"} color="gray.600" _dark={{ color: "gray.400" }}>Compare quotes and availability. Book an appointment instantly online.</Text>
                    </VStack>
                    <VStack>
                        <Flex position="relative" align={"center"} justify={"center"} w={16} h={16} mb={2} bg={"brand.600/30"} rounded="full" color={"brand.200"} _dark={{ bg: "brand.700/30", color: "brand.700"}}>
                             <Icon strokeWidth={2}boxSize={8} as={Wrench} />
                             <Flex top={-2} right={-2} position={"absolute"} align={"center"} justify={"center"} w={8} h={8} rounded={"full"} bg={"orange.500"}> 
                                <Text fontWeight={"bold"} color={"white"}>3</Text>
                             </Flex>
                         </Flex>
                         <Heading color="gray.600" _dark={{ color: "white" }}>Repair</Heading>
                         <Text textAlign={"center"} maxW={"250px"} color="gray.600" _dark={{ color: "gray.400" }}>Drop off your car. Track the repair progress in real-time through the app.</Text>
                    </VStack>
                    <VStack>
                        <Flex position="relative" align={"center"} justify={"center"} w={16} h={16} mb={2} bg={"brand.600/30"} rounded="full" color={"brand.200"} _dark={{ bg: "brand.700/30", color: "brand.700"}}>
                             <Icon strokeWidth={2}boxSize={8} as={CircleCheckBigIcon} />
                             <Flex top={-2} right={-2} position={"absolute"} align={"center"} justify={"center"} w={8} h={8} rounded={"full"} bg={"orange.500"}> 
                                <Text fontWeight={"bold"} color={"white"}>4</Text>
                             </Flex>
                         </Flex>
                         <Heading color="gray.600" _dark={{ color: "white" }}>Done</Heading>
                         <Text textAlign={"center"} maxW={"275px"} color="gray.600" _dark={{ color: "gray.400" }}>Pay securely online and pick up your car. Rate your experience.</Text>
                    </VStack>
                </SimpleGrid>
           </Box>
            <ChakraFooter />
        </Box>

    )
}

export default ChakraHowItWorks;