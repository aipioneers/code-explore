"""Code analysis - languages, metrics, dependencies, patterns."""

from code_explore.analyzer.dependencies import detect_dependencies
from code_explore.analyzer.language import detect_languages
from code_explore.analyzer.metrics import calculate_metrics
from code_explore.analyzer.patterns import detect_patterns

__all__ = [
    "detect_dependencies",
    "detect_languages",
    "calculate_metrics",
    "detect_patterns",
]
