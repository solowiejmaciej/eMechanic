import { Box, Container, Flex, HStack, Button, Image, Heading, SimpleGrid, VStack, Text, Icon, Center, Badge, Link } from "@chakra-ui/react"
import React, { useState, useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import { Quote, Search, Shield, Star, Car, Clock, MapPin, DollarSign, BarChart, ShieldCheck, BarChart3, Circle, Wrench, Facebook, Twitter, Instagram, Linkedin, Phone, Mail} from "lucide-react";
import { transform } from "framer-motion";
import { motion } from "framer-motion";
import logo from '../../assets/logo.png';


const ChakraFooter = () => {
    return(
        
        <Box as="footer" w="full" borderTop="1px solid" borderColor="gray.100" bg="white" _dark={{bgcolor: "whiteAlpha.50", bg: "rgb(25, 36, 54)", color: "white", borderColor: "whiteAlpha.100"}} py={10}>
            <Container maxW="7xl">
                <SimpleGrid columns={{base: 1, lg: 4}} gap={5}>
                        <VStack align="start">
                            <HStack gap={3}>
                                 <Image h={10} w={10} src={logo} rounded="2xl" shadow="lg" />
                                <Heading size="lg" _dark={{color: "white"}}>eMechanic</Heading>
                            </HStack>
                            <Text fontSize={12} fontWeight="semibold"  color="gray.500" pb={2} _dark={{color: "brand.50"}}>
                                Connecting car owners with the best workshops. Reliable repairs, transparent pricing, and hassle-free booking.
                            </Text>
                            <HStack>
                                <Flex bg="brand.50" rounded="full" color="gray.500" _dark={{bg: "whiteAlpha.200", color: "brand.300" }} _hover={{bg: "brand.500", color: "white"}} p={2}> 
                                    <Icon boxSize={4} as={Facebook}/>
                                </Flex>
                                  <Flex bg="brand.50" rounded="full" color="gray.500" _dark={{bg: "whiteAlpha.200", color: "brand.300" }} _hover={{bg: "brand.500", color: "white"}} p={2}> 
                                    <Icon boxSize={4} as={Twitter}/>
                                </Flex>
                                  <Flex bg="brand.50" rounded="full" color="gray.500" _dark={{bg: "whiteAlpha.200", color: "brand.300" }} _hover={{bg: "brand.500", color: "white"}} p={2}> 
                                    <Icon boxSize={4} as={Instagram}/>
                                </Flex>
                                  <Flex bg="brand.50" rounded="full" color="gray.500" _dark={{bg: "whiteAlpha.200", color: "brand.300" }} _hover={{bg: "brand.500", color: "white"}} p={2}> 
                                    <Icon boxSize={4} as={Linkedin}/>
                                </Flex>
                            </HStack>
                        </VStack>
                        <VStack align="start" gap={3}> 
                            <Heading size="sm"  fontWeight="bold">Quick links</Heading>
                            <Link variant="plain" fontWeight="semibold"  color="gray.500" fontSize={12} href="/find-workshop" _hover={{ color: "brand.600", textDecoration: "none" }} _dark={{color: "brand.100"}}>Find a Workshop</Link>
                            <Link variant="plain" fontWeight="semibold"  color="gray.500" fontSize={12} href="/for-mechanics" _hover={{ color: "brand.600", textDecoration: "none" }} _dark={{color: "brand.100"}}>For Mechanics</Link>
                            <Link variant="plain" fontWeight="semibold"  color="gray.500" fontSize={12} href="/how-it-works" _hover={{ color: "brand.600", textDecoration: "none" }} _dark={{color: "brand.100"}}>How It Works</Link>
                            <Link variant="plain" fontWeight="semibold"  color="gray.500" fontSize={12} href="/about" _hover={{ color: "brand.600", textDecoration: "none" }} _dark={{color: "brand.100"}}>About Us</Link>
                        </VStack>
                        <VStack align="start" gap={3}> 
                            <Heading size="sm"  fontWeight="bold">Support</Heading>
                            <Link variant="plain" fontWeight="semibold" color="gray.500" fontSize={12} href="/help" _hover={{ color: "brand.600", textDecoration: "none" }} _dark={{color: "brand.100"}}>Help Center</Link>
                            <Link variant="plain" fontWeight="semibold"  color="gray.500" fontSize={12} href="/terms" _hover={{ color: "brand.600", textDecoration: "none" }} _dark={{color: "brand.100"}}>Terms of Service</Link>
                            <Link variant="plain" fontWeight="semibold"  color="gray.500" fontSize={12} href="/privacy" _hover={{ color: "brand.600", textDecoration: "none" }} _dark={{color: "brand.100"}}>Privacy Policy</Link>
                            <Link variant="plain" fontWeight="semibold"  color="gray.500" fontSize={12} href="/contact" _hover={{ color: "brand.600", textDecoration: "none" }} _dark={{color: "brand.100"}}>Contact Support</Link>
                        </VStack>
                        <VStack align="start" gap={3}> 
                            <Heading size="sm"  fontWeight="bold">Contact Us</Heading>
                                <Flex gap={2}>
                                    <Icon boxSize={4} color="brand.600" as={MapPin}></Icon>
                                <Text fontWeight="semibold"  color="gray.500" fontSize={12} _dark={{color: "brand.100"}}>123 Auto Lane, Mechanic City, MC 12345</Text>
                                </Flex>
                                <Flex gap={2}>
                                    <Icon boxSize={4} color="brand.600" as={Phone}></Icon>
                                <Text fontWeight="semibold"  color="gray.500" fontSize={12} _dark={{color: "brand.100"}}>+1 (555) 123-4567</Text>
                                </Flex>
                                <Flex gap={2}>
                                    <Icon boxSize={4} color="brand.600" as={Mail}></Icon>
                                <Text fontWeight="semibold"  color="gray.500" fontSize={12} _dark={{color: "brand.100"}}>support@emechanic.com</Text>
                                </Flex>
                        </VStack>
                </SimpleGrid>
                <Box borderTopWidth="1px" borderColor="gray.200" pt={8} mt={8} _dark={{borderColor: "gray.700" }}>
                    <Flex direction={{base: "column", md: "row"}} justify="space-between" align="center">
                        <Text fontSize={12} color="gray.500" _dark={{color: "brand.100"}}>
                            © 2026 eMechanic. All rights reserved.
                        </Text>
                        <HStack>
                            <Link variant="plain" color="gray.500" fontSize={13} href="/terms" _hover={{ color: "brand.600", textDecoration: "none" }} _dark={{color: "brand.100"}}>Terms</Link>
                            <Link variant="plain" color="gray.500" fontSize={13} href="/privacy" _hover={{ color: "brand.600", textDecoration: "none" }} _dark={{color: "brand.100"}}>Privacy</Link>
                            <Link variant="plain" color="gray.500" fontSize={13} href="/cookies" _hover={{ color: "brand.600", textDecoration: "none" }} _dark={{color: "brand.100"}}>Cookies</Link>
                        </HStack>
                    </Flex>
                </Box>

            </Container>
        </Box>



)}


export default ChakraFooter;