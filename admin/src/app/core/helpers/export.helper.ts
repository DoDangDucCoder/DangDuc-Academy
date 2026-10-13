export class ExportHelper {
  static exportToCSV(data: any[], filename: string): void {
    if (!data || !data.length) return;
    const headers = Object.keys(data[0]);
    const csvContent = [
      headers.join(','),
      ...data.map(row => headers.map(h => {
        const val = row[h];
        if (val === null || val === undefined) return '';
        if (typeof val === 'object') return `"${JSON.stringify(val).replace(/"/g, '""')}"`;
        if (typeof val === 'string') return `"${val.replace(/"/g, '""')}"`;
        return val;
      }).join(','))
    ].join('\n');
    const blob = new Blob(["\ufeff" + csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `${filename}_${new Date().toISOString().slice(0, 10)}.csv`;
    link.click();
    URL.revokeObjectURL(link.href);
  }
}
