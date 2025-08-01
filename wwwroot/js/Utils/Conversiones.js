function toISODateOrNull(fechaStr) {
    if (!fechaStr) return null;
    const match = fechaStr.match(/(\d{1,2})\/(\d{1,2})\/(\d{4})/);
    if (!match) return null;
    const [_, d, m, y] = match;
    return `${y}-${m.padStart(2, '0')}-${d.padStart(2, '0')}`;
}