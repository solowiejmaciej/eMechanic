import React, { useState, useEffect } from "react";
import {
  Box,
  Flex,
  HStack,
  Button,
  Heading,
  VStack,
  Text,
  Icon,
  Center,
  Skeleton,
  Input,
  MenuRoot,
  MenuTrigger,
  MenuContent,
  MenuItem,
  Portal,
  MenuPositioner,
} from "@chakra-ui/react";
import { ChevronLeft, ChevronRight, Search, ChevronDown } from "lucide-react";

export const DashboardListLayout = ({
  title,
  subtitle,
  extraHeaderAction,
  filters = [], // Array of { id, label, value, onChange, options: [{ value, label, icon, color }] }
  totalItemsLabel,
  currentPage = 1,
  totalPages = 1,
  pageSize = 5,
  onPageChange,
  onPageSizeChange,
  searchPhrase = "",
  onSearchChange,
  loading = false,
  empty = false,
  emptyState,
  children,
}) => {
  // Local state for search phrase
  const [localSearch, setLocalSearch] = useState(searchPhrase);

  // Sync external search phrase changes to local state
  useEffect(() => {
    setLocalSearch(searchPhrase);
  }, [searchPhrase]);

  const handleSearchSubmit = () => {
    if (onSearchChange) {
      onSearchChange(localSearch);
    }
  };

  return (
    <VStack align="stretch" gap={6} w="full">
      
      <Flex justify="space-between" align="center" wrap="wrap" gap={4}>
        <Box>
          <Heading size="2xl" fontWeight="black" tracking="tight" _dark={{ color: "white" }}>
            {title}
          </Heading>
          {subtitle && (
            <Text color="gray.500" _dark={{ color: "gray.400" }} fontSize="md" mt={1}>
              {subtitle}
            </Text>
          )}
        </Box>
        {extraHeaderAction && <Box>{extraHeaderAction}</Box>}
      </Flex>

      
      <Flex
        direction={{ base: "column", md: "row" }}
        justify="space-between"
        align={{ base: "stretch", md: "center" }}
        bg="white"
        _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
        p={4}
        rounded="2xl"
        borderWidth="1px"
        borderColor="gray.200"
        gap={4}
        shadow="sm"
      >
        
        <Flex flex="1" wrap="wrap" gap={4} align="center">
          {onSearchChange && (
            <HStack gap={2} width={{ base: "full", sm: "auto" }}>
              <HStack bg="gray.50" _dark={{ bg: "rgb(15, 23, 42)" }} px={3} py={1.5} rounded="xl" borderWidth="1px" borderColor="gray.250" _darkBorder={{ borderColor: "whiteAlpha.100" }} width={{ base: "full", sm: "240px" }}>
                <Icon as={Search} color="gray.400" boxSize={4} />
                <Input
                  variant="plain"
                  placeholder="Search..."
                  size="sm"
                  value={localSearch}
                  onChange={(e) => setLocalSearch(e.target.value)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter") {
                      handleSearchSubmit();
                    }
                  }}
                  border="none"
                  outline="none"
                  p={0}
                  h="auto"
                  bg="transparent"
                  _dark={{ color: "white" }}
                />
              </HStack>
              <Button
                size="sm"
                colorPalette="orange"
                onClick={handleSearchSubmit}
                rounded="xl"
                fontWeight="semibold"
                px={4}
                color={"white"}
              >
                Search
              </Button>
            </HStack>
          )}

          {filters && filters.length > 0 && (
            <HStack gap={4} wrap="wrap">
              {filters.map((f) => {
                const selectedOpt = f.options.find((opt) => opt.value === f.value) || f.options[0];
                return (
                  <HStack key={f.id} gap={2}>
                    {f.label && (
                      <Text fontSize="sm" fontWeight="bold" color="gray.600" _dark={{ color: "gray.300" }}>
                        {f.label}:
                      </Text>
                    )}
                    <MenuRoot size="sm">
                      <MenuTrigger asChild>
                        <Button
                          variant="outline"
                          size="sm"
                          gap={2.5}
                          width={f.width || "170px"}
                          justifyContent="space-between"
                          bg="white"
                          _dark={{ bg: "rgb(15, 23, 42)", borderColor: "whiteAlpha.100" }}
                          borderColor="gray.250"
                          fontWeight="semibold"
                          rounded="xl"
                        >
                          <HStack gap={1.5}>
                            {selectedOpt?.icon && (
                              <Icon as={selectedOpt.icon} boxSize={3.5} color={selectedOpt.color} />
                            )}
                            <Text truncate>{selectedOpt?.label}</Text>
                          </HStack>
                          <Icon as={ChevronDown} boxSize={3.5} color="gray.400" />
                        </Button>
                      </MenuTrigger>
                      <Portal>
                      <MenuPositioner>
                      <MenuContent
                        bg="white"
                        _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
                        borderColor="gray.200"
                        rounded="xl"
                        shadow="lg"
                        minW={f.width || "170px"}
                      >
                        {f.options.map((opt) => (
                          <MenuItem
                            key={opt.value}
                            value={opt.value}
                            onClick={() => {
                              f.onChange(opt.value);
                              if (onPageChange) onPageChange(1);
                            }}
                            _hover={{ bg: "orange.50", _dark: { bg: "whiteAlpha.100" } }}
                            cursor="pointer"
                            rounded="lg"
                            m={1}
                            px={3}
                            py={1.5}
                            fontSize="sm"
                            display="flex"
                            alignItems="center"
                            gap={2.5}
                          >
                            {opt.icon && (
                              <Icon as={opt.icon} boxSize={3.5} color={opt.color} />
                            )}
                            <Text>{opt.label}</Text>
                          </MenuItem>
                        ))}
                      </MenuContent>
                      </MenuPositioner>
                      </Portal>
                    </MenuRoot>
                  </HStack>
                );
              })}
            </HStack>
          )}
        </Flex>

        {/* Right side: Items counter & Page size select */}
        <HStack gap={4} wrap="wrap" justify={{ base: "flex-start", md: "flex-end" }} align="center">
          {onPageSizeChange && (
            <HStack gap={2}>
              <Text fontSize="xs" fontWeight="bold" color="gray.500" _dark={{ color: "gray.400" }}>
                Records:
              </Text>
              <MenuRoot size="xs">
                <MenuTrigger asChild>
                  <Button
                    variant="outline"
                    size="xs"
                    gap={1.5}
                    width="60px"
                    justifyContent="space-between"
                    bg="white"
                    _dark={{ bg: "rgb(15, 23, 42)", borderColor: "whiteAlpha.100" }}
                    borderColor="gray.250"
                    fontWeight="bold"
                    rounded="lg"
                  >
                    <Text>{pageSize}</Text>
                    <Icon as={ChevronDown} boxSize={3} color="gray.400" />
                  </Button>
                </MenuTrigger>
                <Portal>
                  <MenuPositioner>
                <MenuContent
                  bg="white"
                  _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
                  borderColor="gray.200"
                  rounded="lg"
                  shadow="md"
                  minW="60px"
                >
                  {[5, 10, 20, 50].map((sz) => (
                    <MenuItem
                      key={sz}
                      value={sz.toString()}
                      onClick={() => {
                        onPageSizeChange(sz);
                        if (onPageChange) onPageChange(1);
                      }}
                      _hover={{ bg: "orange.50", _dark: { bg: "whiteAlpha.100" } }}
                      cursor="pointer"
                      rounded="md"
                      m={0.5}
                      px={2}
                      py={1}
                      fontSize="xs"
                      justifyContent="center"
                    >
                      {sz}
                    </MenuItem>
                  ))}
                </MenuContent>
                </MenuPositioner>
                </Portal>
              </MenuRoot>
            </HStack>
          )}
          {totalItemsLabel && (
            <Text fontSize="xs" color="gray.400" fontWeight="bold" textAlign={{ base: "left", md: "right" }}>
              {totalItemsLabel}
            </Text>
          )}
        </HStack>
      </Flex>

      {/* Main Content Area */}
      {loading ? (
        <VStack gap={4} align="stretch" w="full">
          {[...Array(pageSize > 3 ? 3 : pageSize)].map((_, i) => (
            <Box
              key={i}
              p={6}
              bg="white"
              _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
              rounded="2xl"
              borderWidth="1px"
              borderColor="gray.200"
            >
              <Flex justify="space-between" align="center" mb={4}>
                <Skeleton h="20px" w="150px" />
                <Skeleton h="20px" w="100px" />
              </Flex>
              <Skeleton h="60px" w="full" />
            </Box>
          ))}
        </VStack>
      ) : empty ? (
        <Box w="full">{emptyState}</Box>
      ) : (
        <Box w="full">{children}</Box>
      )}

      {/* Reusable Premium Orange Pagination Controls */}
      {!loading && !empty && totalPages > 1 && (
        <Flex
          justify="center"
          align="center"
          gap={4}
          mt={2}
          bg="white"
          _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
          p={3}
          rounded="2xl"
          borderWidth="1px"
          borderColor="gray.200"
          shadow="sm"
        >
          <HStack gap={2}>
            <Button
              variant="ghost"
              size="xs"
              colorPalette="orange"
              disabled={currentPage === 1}
              onClick={() => onPageChange(currentPage - 1)}
              rounded="lg"
              gap={1}
              px={2.5}
            >
              <Icon as={ChevronLeft} boxSize={3.5} />
              Previous
            </Button>
            <Text fontSize="xs" fontWeight="bold" color="gray.700" _dark={{ color: "gray.300" }} px={1}>
              Page {currentPage} z {totalPages}
            </Text>
            <Button
              variant="ghost"
              size="xs"
              colorPalette="orange"
              disabled={currentPage === totalPages}
              onClick={() => onPageChange(currentPage + 1)}
              rounded="lg"
              gap={1}
              px={2.5}
            >
              Next
              <Icon as={ChevronRight} boxSize={3.5} />
            </Button>
          </HStack>
        </Flex>
      )}
    </VStack>
  );
};

export default DashboardListLayout;
