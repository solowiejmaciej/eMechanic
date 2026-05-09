import { Provider } from "@/components/ui/provider"
import { Box, Button, Heading, InputGroup, SimpleGrid, Text, VStack, Flex, Icon, HStack, Collapsible, CollapsibleTrigger, CollapsibleContent, Field, Fieldset, For, Input, NativeSelect, Textarea, Link} from "@chakra-ui/react"
import ChakraNavbar from "@/components/layout/ChakraNavbar"
import ChakraHero from "../features/landing/ChakraHero"
import ChakraFeature from "../features/landing/ChakraFeature"
import ChakraFooter from "../components/layout/ChakraFooter"
import { LuSearch } from "react-icons/lu"
import { BarChart3, Calendar, CircleCheck, CircleCheckBigIcon, CircleQuestionMark, CloudLightning, FileText, Mail, MapPin, MessageCircle, Phone, Pin, Search, TicketX, Users, Wrench, Zap } from "lucide-react"
import { BiQuestionMark } from "react-icons/bi"
import React from "react"

const ChakraForMechanics = () => {
    return(
        <Box display={"flex"} minH="100vh" w="full" bg="gray.100" _dark={{ bg: "#0F172A" }} flexDirection={"column"}>
            <ChakraNavbar />
            <Box flex="1" p={100} maxW="1400px" mx="auto">
                <VStack>
                    <Heading size="6xl" fontWeight={"bold"} _dark={{ color: "white" }}>Grow Your Workshop Business</Heading>
                    <Text maxW={"600px"} fontSize={"lg"} textAlign={"center"} _dark={{ color: "white" }} mt={4}>Join thousands of workshops using eMechanic to streamline operations, attract new customers, and increase revenue.</Text>
                    <Button asChild bg="brand.600" size="2xl" rounded="full" _hover={{ bg: "brand.700" }} className="group" marginTop={30}>
                                <Link to="/find-workshop" color="white" textDecoration={"none"}>
                                    Register Your Workshop
                                </Link>
                    </Button>
                </VStack>
                <SimpleGrid columns={{base: 1, lg: 3}} gap={10} mt={40} >
                    <Box w="full" h="full" minH="120px" display="flex" flexDirection="column" border="sm" rounded="2xl" p={8} boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }} transition="all 0.3 ease">
                    <Flex position="relative" align={"center"} justify={"center"} w={12} h={12} mb={2} bg={"brand.300/20"} rounded="lg" color={"brand.200"} _dark={{ bg: "brand.700/30", color: "brand.700"}}>
                             <Icon strokeWidth={2} boxSize={6} as={Users} color={"brand.500"}/>
                    </Flex>
                    <Heading mt={4}>More Customers</Heading>
                    <Text color="gray.600" _dark={{ color: "gray.400" }} >Get discovered by local car owners actively looking for repair services.</Text>
                    </Box>
                    <Box w="full" h="full" minH="120px" display="flex" flexDirection="column" border="sm" rounded="2xl" p={8} boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }} transition="all 0.3 ease">
                    <Flex position="relative" align={"center"} justify={"center"} w={12} h={12} mb={2} bg="orange.200/30" rounded="lg" _dark={{ bg: "orange.900/50"}}>
                             <Icon strokeWidth={2}boxSize={6} as={Zap} color={"orange.300"} _dark={{color: "orange.400"}}/>
                    </Flex>
                    <Heading mt={4}>More Customers</Heading>
                    <Text color="gray.600" _dark={{ color: "gray.400" }} >Get discovered by local car owners actively looking for repair services.</Text>
                    </Box>
                    <Box w="full" h="full" minH="120px" display="flex" flexDirection="column" border="sm" rounded="2xl" p={8} boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }} transition="all 0.3 ease">
                    <Flex position="relative" align={"center"} justify={"center"} w={12} h={12} mb={2} bg={"green.100"} rounded="lg" color={"brand.200"} _dark={{ bg: "green.700/10"}}>
                             <Icon strokeWidth={2}boxSize={6} as={BarChart3} color={"green.600"} _dark={{color: "green.400"}}/>
                    </Flex>
                    <Heading mt={4}>More Customers</Heading>
                    <Text color="gray.600" _dark={{ color: "gray.400" }} >Get discovered by local car owners actively looking for repair services.</Text>
                    </Box>
                </SimpleGrid>           
           </Box>
            <ChakraFooter />
        </Box>


    )
}


export default ChakraForMechanics;
