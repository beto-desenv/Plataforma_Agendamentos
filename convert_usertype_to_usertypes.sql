-- Script SQL para converter UserType para UserTypes
-- Execute este script no seu banco PostgreSQL

-- 1. Adicionar coluna temporária
ALTER TABLE "Users" ADD COLUMN "UserTypes_temp" TEXT;

-- 2. Converter dados existentes
UPDATE "Users" 
SET "UserTypes_temp" = CASE 
    WHEN "UserType" = 'cliente' THEN '["cliente"]'
    WHEN "UserType" = 'prestador' THEN '["prestador"]'
    ELSE '["cliente"]'
END;

-- 3. Remover coluna antiga
ALTER TABLE "Users" DROP COLUMN "UserType";

-- 4. Renomear coluna temporária
ALTER TABLE "Users" RENAME COLUMN "UserTypes_temp" TO "UserTypes";

-- 5. Tornar NOT NULL
ALTER TABLE "Users" ALTER COLUMN "UserTypes" SET NOT NULL;