// Generates a downloadable PDF for financial pages using jsPDF + jspdf-autotable.
// Loaded once globally; razor pages call window.downloadPdf(data) via IJSRuntime.
window.downloadPdf = function (data) {
    if (typeof window.jspdf === "undefined" || !window.jspdf.jsPDF) {
        alert("PDF library failed to load. Check your internet connection and reload the page.");
        return;
    }

    const { jsPDF } = window.jspdf;
    const doc = new jsPDF({ unit: "pt", format: "a4" });

    const role = data?.role ?? "Financial";
    const generated = data?.generated ?? new Date().toLocaleString();
    const totalIncome = data?.totalIncome ?? "0.00";
    const totalExpenses = data?.totalExpenses ?? "0.00";
    const netPosition = data?.netPosition ?? "0.00";
    const activeBudgets = String(data?.activeBudgets ?? "0");

    const pageWidth = doc.internal.pageSize.getWidth();
    const marginX = 40;
    let cursorY = 50;

    // Title
    doc.setFont("helvetica", "bold");
    doc.setFontSize(18);
    doc.setTextColor(13, 33, 55);
    doc.text(`${role} — Financial Report`, marginX, cursorY);

    cursorY += 16;
    doc.setFont("helvetica", "normal");
    doc.setFontSize(10);
    doc.setTextColor(107, 124, 147);
    doc.text(`Generated: ${generated}`, marginX, cursorY);

    cursorY += 24;

    // Summary cards (rendered as a simple 4-column row)
    const cardWidth = (pageWidth - marginX * 2 - 30) / 4;
    const cards = [
        { label: "Total Income",   value: `$${totalIncome}`,    color: [39, 174, 96] },
        { label: "Total Expenses", value: `$${totalExpenses}`,  color: [192, 57, 43] },
        { label: "Net Position",   value: `$${netPosition}`,    color: [13, 33, 55] },
        { label: "Allocations",    value: activeBudgets,        color: [26, 82, 118] }
    ];
    cards.forEach((c, i) => {
        const x = marginX + i * (cardWidth + 10);
        doc.setDrawColor(232, 236, 241);
        doc.setLineWidth(1);
        doc.roundedRect(x, cursorY, cardWidth, 50, 4, 4, "S");
        doc.setFont("helvetica", "normal");
        doc.setFontSize(8);
        doc.setTextColor(107, 124, 147);
        doc.text(c.label.toUpperCase(), x + 10, cursorY + 14);
        doc.setFont("helvetica", "bold");
        doc.setFontSize(14);
        doc.setTextColor(c.color[0], c.color[1], c.color[2]);
        doc.text(c.value, x + 10, cursorY + 36);
    });

    cursorY += 70;

    // Allocations table
    const allocations = data?.allocations ?? [];
    const showUtilization = allocations.some(a => a.utilization !== undefined && a.utilization !== null);
    const allocHead = ["Name", "Level", "Allocated", "Spent", "Remaining"];
    if (showUtilization) allocHead.push("Utilization");

    const allocBody = allocations.map(a => {
        const row = [
            a.name ?? "",
            a.level ?? "",
            `$${a.allocated ?? ""}`,
            `$${a.spent ?? ""}`,
            `$${a.remaining ?? ""}`
        ];
        if (showUtilization) row.push(`${a.utilization ?? "0"}%`);
        return row;
    });

    doc.setFont("helvetica", "bold");
    doc.setFontSize(11);
    doc.setTextColor(13, 33, 55);
    doc.text("Budget Allocations", marginX, cursorY);
    cursorY += 6;

    doc.autoTable({
        startY: cursorY,
        head: [allocHead],
        body: allocBody.length ? allocBody : [["—", "", "", "", "", ...(showUtilization ? [""] : [])].slice(0, allocHead.length)],
        margin: { left: marginX, right: marginX },
        styles: { fontSize: 9, cellPadding: 5 },
        headStyles: { fillColor: [13, 33, 55], textColor: 255, fontStyle: "bold", fontSize: 9 },
        alternateRowStyles: { fillColor: [248, 250, 252] },
        columnStyles: {
            2: { halign: "right" },
            3: { halign: "right" },
            4: { halign: "right" },
            5: { halign: "right" }
        },
        didParseCell: (hookData) => {
            if (hookData.section === "body" && hookData.column.index === 4) {
                hookData.cell.styles.fontStyle = "bold";
            }
        }
    });

    cursorY = doc.lastAutoTable.finalY + 22;

    // Transactions table
    const transactions = data?.transactions ?? [];
    const showBudget = transactions.some(t => t.budget !== undefined && t.budget !== null && t.budget !== "");

    const txHead = ["Date", "Description", "Allocation"];
    if (showBudget) txHead.push("Budget");
    txHead.push("Type", "Amount");

    const txBody = transactions.map(t => {
        const row = [
            t.date ?? "",
            t.description ?? "",
            t.allocation ?? "—"
        ];
        if (showBudget) row.push(t.budget ?? "—");
        row.push((t.type ?? "").toUpperCase());
        const sign = t.isIncome ? "+" : "-";
        row.push(`${sign}$${t.amount ?? "0"}`);
        return row;
    });

    // If the next section won't fit, page-break before drawing the header.
    if (cursorY > doc.internal.pageSize.getHeight() - 100) {
        doc.addPage();
        cursorY = 50;
    }

    doc.setFont("helvetica", "bold");
    doc.setFontSize(11);
    doc.setTextColor(13, 33, 55);
    doc.text("Transactions", marginX, cursorY);
    cursorY += 6;

    doc.autoTable({
        startY: cursorY,
        head: [txHead],
        body: txBody.length ? txBody : [Array(txHead.length).fill("—")],
        margin: { left: marginX, right: marginX },
        styles: { fontSize: 9, cellPadding: 5 },
        headStyles: { fillColor: [13, 33, 55], textColor: 255, fontStyle: "bold", fontSize: 9 },
        alternateRowStyles: { fillColor: [248, 250, 252] },
        columnStyles: {
            [txHead.length - 1]: { halign: "right", fontStyle: "bold" }
        },
        didParseCell: (hookData) => {
            if (hookData.section === "body" && hookData.column.index === txHead.length - 1) {
                const cellText = hookData.cell.raw ?? "";
                if (typeof cellText === "string" && cellText.startsWith("+")) {
                    hookData.cell.styles.textColor = [30, 132, 73];
                } else if (typeof cellText === "string" && cellText.startsWith("-")) {
                    hookData.cell.styles.textColor = [192, 57, 43];
                }
            }
            if (hookData.section === "body" && hookData.column.index === txHead.length - 2) {
                const cellText = String(hookData.cell.raw ?? "");
                if (cellText === "INCOME") {
                    hookData.cell.styles.fillColor = [234, 250, 241];
                    hookData.cell.styles.textColor = [30, 132, 73];
                    hookData.cell.styles.fontStyle = "bold";
                } else if (cellText === "EXPENSE") {
                    hookData.cell.styles.fillColor = [253, 237, 236];
                    hookData.cell.styles.textColor = [192, 57, 43];
                    hookData.cell.styles.fontStyle = "bold";
                }
            }
        }
    });

    // Page numbers footer
    const pageCount = doc.internal.getNumberOfPages();
    for (let i = 1; i <= pageCount; i++) {
        doc.setPage(i);
        doc.setFont("helvetica", "normal");
        doc.setFontSize(8);
        doc.setTextColor(142, 153, 164);
        doc.text(
            `${role} — Page ${i} of ${pageCount}`,
            pageWidth / 2,
            doc.internal.pageSize.getHeight() - 20,
            { align: "center" }
        );
    }

    const safeRole = role.replace(/[^a-z0-9]+/gi, "_").replace(/^_|_$/g, "");
    const stamp = new Date().toISOString().slice(0, 10);
    doc.save(`${safeRole}_financial_report_${stamp}.pdf`);
};
