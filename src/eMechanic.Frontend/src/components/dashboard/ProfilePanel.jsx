import React from "react";
import {
  Box,
  Flex,
  Button,
  Heading,
  SimpleGrid,
  VStack,
  Text,
  Input,
  Separator,
  Field,
} from "@chakra-ui/react";

export const ProfilePanel = ({
  firstName,
  setFirstName,
  lastName,
  setLastName,
  email,
  setEmail,
  phone,
  setPhone,
  submitting,
  onSubmit,
}) => {
  return (
    <VStack gap={6} align="stretch">
      <Box>
        <Heading size="2xl" fontWeight="black" tracking="tight" _dark={{ color: "white" }}>
          My profile
        </Heading>
        <Text color="gray.500" _dark={{ color: "gray.400" }} fontSize="md" mt={1}>
          Update your personal information and contact details.
        </Text>
      </Box>

      <Box
        p={6}
        bg="white"
        _dark={{ bg: "rgb(25, 36, 54)", borderColor: "whiteAlpha.100" }}
        borderWidth="1px"
        borderColor="gray.200"
        rounded="2xl"
        boxShadow="0 15px 35px -5px rgba(59, 130, 246, 0.06)"
        as="form"
        onSubmit={onSubmit}
      >
        <VStack align="stretch" gap={5}>
          <SimpleGrid columns={{ base: 1, md: 2 }} gap={4}>
            <Field.Root required>
              <Field.Label fontWeight="semibold">Name</Field.Label>
              <Input
                placeholder="ex. Jan"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
                _dark={{ bg: "rgb(15, 23, 42)" }}
                rounded="xl"
                size="md"
                _focus={{
                  borderColor: "brand.500",
                  borderWidth: "2px",
                  outline: "none",
                }}
              />
            </Field.Root>

            <Field.Root required>
              <Field.Label fontWeight="semibold">Last name</Field.Label>
              <Input
                placeholder="ex. Kowalski"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
                _dark={{ bg: "rgb(15, 23, 42)" }}
                rounded="xl"
                size="md"
                _focus={{
                  borderColor: "brand.500",
                  borderWidth: "2px",
                  outline: "none",
                }}
              />
            </Field.Root>
          </SimpleGrid>

          <SimpleGrid columns={{ base: 1, md: 2 }} gap={4}>
            <Field.Root required>
              <Field.Label fontWeight="semibold">E-mail address</Field.Label>
              <Input
                type="email"
                placeholder="ex. jan.kowalski@email.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                _dark={{ bg: "rgb(15, 23, 42)" }}
                rounded="xl"
                size="md"
                _focus={{
                  borderColor: "brand.500",
                  borderWidth: "2px",
                  outline: "none",
                }}
              />
            </Field.Root>

            <Field.Root>
              <Field.Label fontWeight="semibold">Phone number</Field.Label>
              <Input
                placeholder="ex. +48500100200"
                value={phone}
                onChange={(e) => setPhone(e.target.value)}
                _dark={{ bg: "rgb(15, 23, 42)" }}
                rounded="xl"
                size="md"
                _focus={{
                  borderColor: "brand.500",
                  borderWidth: "2px",
                  outline: "none",
                }}
              />
            </Field.Root>
          </SimpleGrid>

          <Separator borderColor="gray.100" _dark={{ borderColor: "whiteAlpha.100" }} mt={2} />

          <Flex justify="flex-end">
            <Button
              type="submit"
              colorPalette="brand"
              loading={submitting}
              rounded="xl"
              px={8}
              fontWeight="bold"
              shadow="md"
              _hover={{ transform: "translateY(-1px)", shadow: "lg" }}
            >
              Save changes
            </Button>
          </Flex>
        </VStack>
      </Box>
    </VStack>
  );
};

export default ProfilePanel;
