import { Box, Container, Flex, HStack, Button, Image, Heading, SimpleGrid, VStack, Text, Icon, Center, Badge } from "@chakra-ui/react"
import React, { useState, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Quote, Search, Shield, Star, Car, Clock, MapPin} from "lucide-react";
import { transform } from "framer-motion";
import { motion } from "framer-motion";

const MotionBox = motion(Box);
const MotionVStack = motion(VStack);
const MotionText = motion(Text);
const MotionHeading = motion(Heading);


const containerVariants = {
  hidden: { opacity: 0 },
  visible: {
    opacity: 1,
    transition: {
      staggerChildren: 0.15, 
      delayChildren: 0.1,
    },
  },
};

const itemVariants = {
  hidden: { opacity: 0, y: 20, filter: "blur(10px)" },
  visible: { 
    opacity: 1, 
    y: 0, 
    filter: "blur(0px)",
    transition: { duration: 0.6, ease: "easeOut" } 
  },
};


const imageVariants = {
  hidden: { opacity: 0, x: 100 },
  visible: { 
    opacity: 1, 
    x: 0,
    transition: { duration: 0.8, ease: "easeOut", delay: 0.5 } 
  },
  float: { 
    y: [0, -20, 0],
    rotate: [0, 0.01, 0],
    transition: {
      duration: 6,
      repeat: Infinity,
      ease: "easeInOut" 
    }
  }
};


const ChakraHero = () => {
    return(
        <Box>
            <Container maxW="7xl">
                <SimpleGrid columns={{base : 1, lg: 2}}>
                    <MotionVStack variants={containerVariants} initial="hidden" animate="visible" p="20px" align="start">
                        <MotionBox variants={itemVariants}>
                            <HStack bg="white" rounded="full" pt="5px" pb="5px" pl="10px" pr="10px" shadow="sm">
                                <Text fontWeight="medium" textStyle="sm" color="blue.500">
                                    #1 Trusted Mechanic Network
                                </Text>
                            </HStack>
                        </MotionBox>
                        <MotionBox variants={itemVariants}>
                            <Heading size={{base : "lg", lg : "7xl"}} fontWeight="bold" lineHeight="1.2" _dark={{color: "white"}}>
                                Car Repair
                                <Text size={{base : "lg", lg : "7xl"}} color="transparent" bgClip="text" backgroundImage="linear-gradient(to right, var(--chakra-colors-brand-600), var(--chakra-colors-brand-300))" >
                                Made Simple.
                            </Text>   
                            </Heading>
                        </MotionBox>
                        <MotionBox variants={itemVariants}>
                            <Text color="gray.900" marginTop="16px" _dark={{color: "white"}}>
                                Connect with top-rated local mechanics instantly. Compare quotes, book appointments, and track repairs - all in one place.
                            </Text>
                        </MotionBox>
                        <MotionBox pt={4} variants={itemVariants}>
                            <Button asChild bg="brand.600" size="2xl" rounded="full" _hover={{bg: "brand.700" }} className="group" marginTop={30}>
                                <Link to="/find-workshop">
                                Find a Workshop
                                <Icon as={Search} size="sm" transition="transform 0.2s" _groupHover={{ transform : "translate(5px)"}}>
                                </Icon>
                                </Link>
                            </Button>
                        </MotionBox>
                        <MotionBox variants={itemVariants}>
                            <HStack marginTop="10">
                                <Icon as={Shield} color="brand.600" size="sm"> </Icon>
                                <Text color="gray.500" textStyle="sm">Verified Pros</Text>
                                <Icon as={Star} color="orange" size="sm"> </Icon>
                                <Text color="gray.500" textStyle="sm">4.96/5 Rating</Text>
                                <Icon as={Quote} color="brand.600" size="sm"> </Icon>
                                <Text color="gray.500" textStyle="sm">Instant Quotes</Text>
                            </HStack>
                        </MotionBox>
                    </MotionVStack>
                    <Flex position="relative" align="center" justify="center">
                        <Box position="absolute" bg="brand.600" w="300px" h="300px" rounded="full" top="-10%" right="-10%" opacity="0.6" filter="blur(70px)">
                        </Box>
                        <Box position="absolute" bg="orange" w="300px" h="300px" rounded="full" bottom="-10%" left="-10%" opacity="0.6" filter="blur(70px)">
                        </Box>
                        <MotionBox bg="white" rounded="2xl" shadow="2xl" border="1px solid" borderColor="gray.100" p="20px" w="full" maxW="600px" zIndex="10" _dark={{bg: "rgb(25, 36, 54)", borderColor: "rgba(30, 41, 59, 0.5)"}} variants={imageVariants} initial="hidden" animate={["visible", "float"]} style={{willChange: "transform", backfaceVisibility: "hidden", perspective: "1000"}} transformTemplate={({ y }) => `translate3d(0, ${y || 0}, 0)`} >
                            <VStack>
                                <HStack gap="4" w="full" mb="6" pb="6" borderBottom="1px solid" borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }}>
                                    <Flex w="12" h="12" bg="brand.100" rounded="full" align="center" justify="center" _dark={{bg: "brand.600"}}>
                                        <Icon as={Car} color="brand.400"/>
                                    </Flex>
                                    <Box>
                                        <Text fontWeight="bold" fontSize="lg" color="gray.900" _dark={{color: "white"}}>
                                            Vechicle Status
                                        </Text>
                                        <Text fontSize="sm" color="gray.500" _dark={{color: "white"}}>
                                            Opel Astra K • 2018
                                        </Text>
                                    </Box>
                                    <Badge ml="auto" colorPalette="green" rounded="2xl" pt={1} pb={1} pl={2} pr={2} fontWeight="bold">
                                        Active
                                    </Badge>
                                </HStack>
                                <Box w="full" bg="gray.50" p="5" rounded="2xl" _dark={{bg: "rgba(15, 23, 42, 0.9)", borderColor: "rgba(30, 41, 59, 0.5)"}}>
                                    <Flex justify="space-between">
                                        <Text pb="3" color="gray.600" textStyle="sm" _dark={{color: "white"}}>Oil change</Text>
                                        <Text pb="3" textStyle="sm" fontWeight="bold" _dark={{color: "white"}}>$89.00</Text>
                                    </Flex>
                                        <Box w="full" bg="gray.300" rounded="full" h="10px" overflow="hidden"> 
                                           <MotionBox bg="brand.600" animate={{width: "75%"}} rounded="full" initial={{width: "0"}} h="100%">

                                           </MotionBox>
                                        </Box>
                                </Box>
                                <Box w="full" bg="gray.50" p="5" rounded="2xl" _dark={{bg: "rgba(15, 23, 42, 0.9)", borderColor: "rgba(77, 75, 75, 0.5)"}}>
                                    <Flex justify="space-between">
                                        <Text pb="3" color="gray.600" textStyle="sm" _dark={{color: "white"}}>Brake Inspection</Text>
                                        <Text pb="3" textStyle="sm" fontWeight="bold" _dark={{color: "white"}}>Pending</Text>
                                    </Flex>
                                        <Box w="full" bg="gray.300" rounded="full" h="10px" overflow="hidden"> 
                                           <MotionBox bg="orange.600" animate={{width: "50%"}} rounded="full" initial={{width: "0"}} h="100%">
                                           </MotionBox>
                                        </Box>
                                </Box>
                                <SimpleGrid columns={2} w="full" gap="32px">
                                    <Box p={10} rounded="xl" bg="brand.50">
                                    <VStack>
                                        <Icon as={Clock} size="lg" color="brand.600"/>
                                        <Text textStyle="2xl" fontWeight="bold" color="brand.600">
                                            24h
                                        </Text>
                                    <Text fontSize="xs" fontWeight="medium" color="gray.500">Avg. Turnaround</Text>
                                    </VStack>
                                </Box>
                                <Box p={10} rounded="xl" bg="orange.50">
                                    <VStack>
                                        <Icon as={MapPin} size="lg" color="orange.600"/>
                                        <Text textStyle="2xl" fontWeight="bold" color="orange.600">
                                            15+
                                        </Text>
                                    <Text fontSize="xs" fontWeight="medium" color="gray.500">Local Shops</Text>
                                    </VStack>
                                </Box>
                                </SimpleGrid>
                            </VStack>
                        </MotionBox>
                    </Flex>
                </SimpleGrid>
            </Container>
        </Box>
    )
}


export default ChakraHero;