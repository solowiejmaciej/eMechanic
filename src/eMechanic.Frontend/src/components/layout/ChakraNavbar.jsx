import { Box, Container, Flex, HStack, Button, Image, Heading, IconButton, Icon } from "@chakra-ui/react"
import React, { useState, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { motion } from 'framer-motion';
import ThemeToggle from '../ui/ThemeToggle';
import logo from '../../assets/logo.png';
import { useAuth } from '../../context/AuthContext';
import { Cog } from "lucide-react";

const ChakraNavbar = () => {
    const [isScrolled, setIsScrolled] = useState(false);
    const [isCollapsed, setIsCollapsed] = useState(false);
    const [lastScrollY, setLastScrollY] = useState(0);
    const [manuallyExpanded, setManuallyExpanded] = useState(false);
    const location = useLocation();
    const { user, logout } = useAuth();


    useEffect(() => {
        const handleScroll = () => {
            const currentScrollY = window.scrollY;

            setIsScrolled(currentScrollY > 20);

            if (manuallyExpanded && currentScrollY !== lastScrollY) {
                setIsCollapsed(true);
                setManuallyExpanded(false);
            }
            if (currentScrollY < 20) {
                setIsCollapsed(false);
                setManuallyExpanded(false);
            } else if (currentScrollY > lastScrollY && currentScrollY > 100) {
                setIsCollapsed(true);
                setManuallyExpanded(false);
            }
            setLastScrollY(currentScrollY);
        };

        window.addEventListener('scroll', handleScroll);
        return () => window.removeEventListener('scroll', handleScroll);
    }, [lastScrollY, manuallyExpanded]);

    const isHomePage = location.pathname === '/';
    const showBg = isScrolled || !isHomePage;

    const homePath = user ? "/dashboard" : "/";
    const userColor = user?.role === 'workshop' ? "orange.500" : "brand.500";
    const iconHoverColor = user?.role === 'workshop' ? "orange.400" : "brand.400";
    return (
        <Box
            as={motion.nav}
            animate={{ height: isCollapsed ? '60px' : 'auto' }}
            transition={{ duration: 0.2 }}
            position="fixed"
            top="0"
            w="full"
            zIndex="100"
            py={isCollapsed ? 2 : 4}

            css={{ transition: "background-color 0.3s ease, border-color 0.3s ease, backdrop-filter 0.3s ease" }}


            backdropFilter={showBg ? "blur(12px)" : "none"}
            borderColor="gray.200"


            _dark={{
                bg: showBg ? "rgba(15, 23, 42)" : "transparent",
                borderColor: "rgba(30, 41, 59, 0.5)"
            }}
        >
            <Container maxW="7xl" px={{ base: 4, sm: 6, lg: 8 }}>
                <Flex justify="space-between" align="center" h={16}>
                    <HStack>
                        <Link to={homePath}>
                            <Image h={14} w={14} src={logo} rounded="2xl" shadow="lg" />
                        </Link>
                        <Link to={homePath}>
                            <Heading
                                size="2xl"
                                fontWeight="bold"
                                color={{ base: "gray.900", _dark: "white" }}
                            >
                                eMechanic
                            </Heading>
                        </Link>
                    </HStack>

                    <HStack gap={3}>
                        <ThemeToggle />
                        {user ? (

                            <>
                                <IconButton p={1} rounded="full" bg={userColor} color="slate.600" position="relative" overflow="hidden" transition="all 0.2s" _hover={{ bg: iconHoverColor }} _dark={{ color: "white" }}>
                                    <Icon as={Cog} />
                                </IconButton>
                                <Button bg={userColor} onClick={logout} _dark={{ color: "white" }} colorPalette="brand" rounded="full" variant="solid" _hover={{ bg: iconHoverColor }}>
                                    Log out
                                </Button>
                            </>

                        ) :

                            (
                                <>
                                    <Button
                                        asChild
                                        rounded="full"
                                        variant="ghost"
                                        color="gray.600"
                                        bg="transparent"
                                        _hover={{ color: "brand.600", bg: "transparent" }}
                                        _dark={{
                                            color: "gray.300",
                                            bg: "transparent",
                                            _hover: { color: "brand.400", bg: "transparent" }
                                        }}
                                        _active={{ bg: "transparent" }}
                                    >
                                        <Link to="/login">Log In</Link>
                                    </Button>

                                    <Button asChild colorPalette="brand" rounded="full" variant="solid">
                                        <Link to="/register">Get Started</Link>
                                    </Button>
                                </>

                            )



                        }




                    </HStack>
                </Flex>
            </Container>
        </Box>
    );
};

export default ChakraNavbar;