import { Box, Container, Flex, HStack, Button, Image, Heading, SimpleGrid, VStack, Text, Icon, Center, Badge, InputGroup, Input, Separator } from "@chakra-ui/react"
import React, { useState, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Quote, Search, Shield, Star, Car, Clock, MapPin, Plus, Wrench, Calendar, Filter } from "lucide-react";
import ChakraNavbar from "@/components/layout/ChakraNavbar";
import ChakraFooter from "@/components/layout/ChakraFooter";
import CustomLoader from "@/components/ui/CustomLoader";




const UserDashboard = () => {
    return (

        <Box minH="100vh" w="full" bg="gray.100" _dark={{ bg: "#0F172A" }} display={"flex"} flexDirection={"column"}>
         {/* <ChakraNavbar /> */}
            <Box flex="1" w={"full"}  maxW="1400px" mx="auto" p={20}>
                <Flex justify={"space-between"} w={"full"} align={"center"} mx={"auto"}>
                    <Box>
                        <Heading size="3xl" _dark={{ color: "white" }}>My Garage</Heading>
                        <Text fontSize="lg" _dark={{ color: "whiteAlpha.800" }}> Manage your vehicles and repair history</Text>
                    </Box>

                    <Button asChild colorPalette="brand" rounded="lg" variant="solid">
                        <Link to="/register">
                            <Icon as={Plus} />
                            Add Vehicle</Link>
                    </Button>
                </Flex>
                <SimpleGrid w={"full"} columns={"3"} py={10} gap={10}>
                    <Box h="full" display="flex" flexDirection="column" border="sm" rounded="2xl" p={4} boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }} transition="all 0.3 ease">
                        <Flex align="center" gap={3}>
                            <Flex w={14} h={14} bg="blue.50" rounded="lg" _dark={{ bg: "blue.900" }} align={"center"} justify={"center"}>
                                <Icon as={Car} color="blue.600" boxSize={6} _dark={{ color: "brand.200" }} />
                            </Flex>
                            <VStack align={"flex-start"} justify={"center"} gap={0}>
                                <Text fontSize="sm" _dark={{ color: "whiteAlpha.800" }}> Total Vehicles</Text>
                                <Text fontSize="3xl" fontWeight={"bold"} _dark={{ color: "white" }}>0</Text>
                            </VStack>

                        </Flex>

                    </Box>
                    <Box h="full" display="flex" flexDirection="column" border="sm" rounded="2xl" p={4} boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }} transition="all 0.3 ease">
                        <Flex align="center" gap={3}>
                            <Flex w={14} h={14} bg="orange.50" rounded="lg" _dark={{ bg: "orange.600/30" }} align={"center"} justify={"center"}>
                                <Icon as={Wrench} color="orange.500" boxSize={6} _dark={{ color: "brand.200" }} />
                            </Flex>
                            <VStack align={"flex-start"} justify={"center"} gap={0}>
                                <Text fontSize="sm" _dark={{ color: "whiteAlpha.800" }}>Active Repairs</Text>
                                <Text fontSize="3xl" fontWeight={"bold"} _dark={{ color: "white" }}>0</Text>
                            </VStack>

                        </Flex>

                    </Box>
                    <Box h="full" display="flex" flexDirection="column" border="sm" rounded="2xl" p={4} boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }} transition="all 0.3 ease">
                        <Flex align="center" gap={3}>
                            <Flex w={14} h={14} bg="green.50" rounded="lg" _dark={{ bg: "green.600/20" }} align={"center"} justify={"center"}>
                                <Icon as={Calendar} color="green.500" boxSize={6} _dark={{ color: "brand.200" }} />
                            </Flex>
                            <VStack align={"flex-start"} justify={"center"} gap={0}>
                                <Text fontSize="sm" _dark={{ color: "whiteAlpha.800" }}>Upcoming Visits</Text>
                                <Text fontSize="3xl" fontWeight={"bold"} _dark={{ color: "white" }}>0</Text>
                            </VStack>

                        </Flex>

                    </Box>
                </SimpleGrid>
                <Box h="full" minH={"300px"} display="flex" flexDirection="column" border="sm" rounded="2xl" p={4} boxShadow="lg" borderColor="gray.100" bg="gray.50" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100" }} transition="all 0.3 ease">

                    <Flex justify={"space-between"} w={"full"} align={"center"} mx={"auto"}>
                        <Heading>Vehicles</Heading>
                        <InputGroup maxW={"300px"} flex="1" startElement={<Icon boxSize={5} as={Search} />} >
                            <Input placeholder="Search vehicles" _focus={{ borderColor: "brand.600", borderWidth: "2px", outline: "none", _dark: { borderColor: "brand.600" } }} w="full" rounded="2xl" size="lg" _dark={{ bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }} />
                        </InputGroup>

                    </Flex>
                    <Separator borderColor={"whiteAlpha.100"} mt={5} />
                    <CustomLoader />
                </Box>

            </Box>
            <ChakraFooter />
        </Box>




    )

}

export default UserDashboard;