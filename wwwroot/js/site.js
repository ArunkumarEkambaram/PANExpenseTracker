// ── Modal helpers ──────────────────────────────────────────
function openModal(id) {
    document.getElementById(id)?.classList.add('open');
}
function closeModal(id) {
    document.getElementById(id)?.classList.remove('open');
}
// Close on overlay click
document.addEventListener('click', e => {
    if (e.target.classList.contains('modal-overlay')) {
        e.target.classList.remove('open');
    }
});
// Close on Escape
document.addEventListener('keydown', e => {
    if (e.key === 'Escape') {
        document.querySelectorAll('.modal-overlay.open').forEach(m => m.classList.remove('open'));
    }
});

// ── Filter buttons ─────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.filter-btn[data-period]').forEach(btn => {
        btn.addEventListener('click', () => {
            const period = btn.dataset.period;
            const url = new URL(window.location.href);
            url.searchParams.set('period', period);
            if (period !== 'custom') {
                url.searchParams.delete('from');
                url.searchParams.delete('to');
            }
            window.location.href = url.toString();
        });
    });

    // Highlight active filter
    const params = new URLSearchParams(window.location.search);
    const period = params.get('period') || 'month';
    document.querySelectorAll('.filter-btn[data-period]').forEach(btn => {
        btn.classList.toggle('active', btn.dataset.period === period);
    });

    // Custom date range
    const applyCustom = document.getElementById('applyCustomRange');
    if (applyCustom) {
        applyCustom.addEventListener('click', () => {
            const from = document.getElementById('customFrom')?.value;
            const to = document.getElementById('customTo')?.value;
            if (!from || !to) { alert('Please select both dates.'); return; }
            const url = new URL(window.location.href);
            url.searchParams.set('period', 'custom');
            url.searchParams.set('from', from);
            url.searchParams.set('to', to);
            window.location.href = url.toString();
        });
    }

    // Auto-dismiss alerts
    setTimeout(() => {
        document.querySelectorAll('.alert').forEach(a => {
            a.style.transition = 'opacity 0.5s';
            a.style.opacity = '0';
            setTimeout(() => a.remove(), 500);
        });
    }, 3500);

    // Animate progress bars
    document.querySelectorAll('.progress-fill, .chart-bar-fill').forEach(el => {
        const target = el.dataset.width || el.style.width;
        el.style.width = '0';
        setTimeout(() => { el.style.width = target; }, 100);
    });

    // Animate trend bars
    document.querySelectorAll('.trend-bar-inc, .trend-bar-exp').forEach(el => {
        const target = el.dataset.height || el.style.height;
        el.style.height = '0';
        setTimeout(() => { el.style.height = target; }, 100);
    });
});

// ── Format currency ────────────────────────────────────────
function formatINR(amount) {
    return '₹' + Number(amount).toLocaleString('en-IN', { minimumFractionDigits: 2 });
}

// ── Confirm delete ─────────────────────────────────────────
function confirmDelete(formId) {
    if (confirm('Are you sure you want to delete this record? It will be soft-deleted and can be recovered.')) {
        document.getElementById(formId).submit();
    }
}

// ── Loan interest calculator ────────────────────────────────
function calcLoanInterest() {
    const amount = parseFloat(document.getElementById('loanAmount')?.value) || 0;
    const percentage = parseFloat(document.getElementById('loanInterestPercentage')?.value) || 0;
    const interestType = document.getElementById('loanInterestType')?.value;
    const givenDate = new Date(document.getElementById('loanGivenDate')?.value);
    const today = new Date();

    if (!amount || !givenDate || isNaN(givenDate)) return;

    // Auto-calculate interest amount from percentage
    let interestPerPeriod = 0;
    if (interestType === 'Monthly') {
        interestPerPeriod = (amount * percentage) / 100;
    } else {
        interestPerPeriod = ((amount * percentage) / 100) * 12;
    }
    const interestAmtEl = document.getElementById('loanInterest');
    if (interestAmtEl) interestAmtEl.value = interestPerPeriod.toFixed(2);

    // Recalculate total due
    const days = Math.max(0, (today - givenDate) / (1000 * 60 * 60 * 24));
    const months = days / 30.44;
    let totalInterest = 0;
    if (interestType === 'Monthly') {
        totalInterest = interestPerPeriod * Math.floor(months);
    } else {
        totalInterest = interestPerPeriod * Math.floor(months / 12);
    }
    const totalDue = amount + totalInterest;

    // Update Interest Amount textbox based on type
    document.getElementById('loanInterestAmount').value = interestPerPeriod.toFixed(2);

    const el = document.getElementById('loanCalcResult');
    if (el) {
        el.innerHTML = `
            <div class="loan-calc">
                <div class="row"><span>Principal</span><span>₹${amount.toLocaleString('en-IN', { minimumFractionDigits: 2 })}</span></div>
                <div class="row"><span>Interest Rate</span><span>${percentage}% / ${interestType}</span></div>
                <div class="row"><span>Interest Per Period</span><span>₹${interestPerPeriod.toLocaleString('en-IN', { minimumFractionDigits: 2 })}</span></div>
                <div class="row"><span>Months Elapsed</span><span>${Math.floor(months)}</span></div>
                <div class="row"><span>Interest Accrued</span><span>₹${totalInterest.toLocaleString('en-IN', { minimumFractionDigits: 2 })}</span></div>
                <div class="row"><span><strong>Total Repayable</strong></span><span><strong>₹${totalDue.toLocaleString('en-IN', { minimumFractionDigits: 2 })}</strong></span></div>
            </div>`;
    }
}