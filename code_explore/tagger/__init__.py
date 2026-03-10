"""AI tag normalization and parsing utilities."""

import re

from code_explore.models import AiTag, TagCategory


def normalize_tag(raw: str) -> str:
    """Normalize a raw tag string to lowercase-hyphenated format."""
    tag = raw.lower().strip()
    tag = re.sub(r"[^a-z0-9-]", "-", tag)
    tag = re.sub(r"-+", "-", tag)
    tag = tag.strip("-")
    return tag


def _parse_category(raw: str) -> TagCategory:
    """Map a raw category string to TagCategory enum."""
    raw = raw.lower().strip()
    mapping = {
        "domain": TagCategory.DOMAIN,
        "technology-role": TagCategory.TECHNOLOGY_ROLE,
        "tech-role": TagCategory.TECHNOLOGY_ROLE,
        "role": TagCategory.TECHNOLOGY_ROLE,
        "maturity": TagCategory.MATURITY,
    }
    return mapping.get(raw, TagCategory.DOMAIN)


def parse_ai_tags(text: str) -> list[AiTag]:
    """Parse AI_TAGS output from Ollama response.

    Expected format per line: category:tag-value
    Example: domain:web-backend, role:api-gateway, maturity:production-ready
    """
    tags: list[AiTag] = []
    seen: set[str] = set()

    for item in text.split(","):
        item = item.strip()
        if not item:
            continue

        if ":" in item:
            category_str, value_str = item.split(":", 1)
            category = _parse_category(category_str)
            value = normalize_tag(value_str)
        else:
            category = TagCategory.DOMAIN
            value = normalize_tag(item)

        if value and value not in seen:
            seen.add(value)
            tags.append(AiTag(value=value, category=category, confidence=0.8))

    return tags
