// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Bildirimleri yükle
function bildirimleriYukle() {
  $.get('/Bildirim/Listele', function (data) {
    const liste = $('#bildirimListesi');
    liste.empty();

    if (data.length === 0) {
      liste.append('<div class="dropdown-item text-muted">Bildirim yok</div>');
      $('#bildirimSayisi').hide();
    } else {
      const okunmamisSayisi = data.filter(b => !b.okundu).length;
      if (okunmamisSayisi > 0) {
        $('#bildirimSayisi').text(okunmamisSayisi).show();
      } else {
        $('#bildirimSayisi').hide();
      }

      data.forEach(bildirim => {
        const item = $(`
                    <div class="dropdown-item ${bildirim.okundu ? 'text-muted' : ''}" data-id="${bildirim.bildirimID}">
                        <div class="d-flex justify-content-between align-items-center">
                            <div>${bildirim.mesaj}</div>
                            <button class="btn btn-sm yildiz-btn ${bildirim.yildizli ? 'text-warning' : 'text-secondary'}">
                                <i class="fas fa-star"></i>
                            </button>
                        </div>
                        <small class="text-muted d-block">
                            ${new Date(bildirim.olusturulmaTarihi).toLocaleString()}
                        </small>
                    </div>
                `);
        liste.append(item);
      });
    }
  });
}

// Bildirimi yıldızla/yıldızı kaldır
$(document).on('click', '.yildiz-btn', function (e) {
  e.stopPropagation();
  const btn = $(this);
  const bildirimId = btn.closest('.dropdown-item').data('id');

  $.post('/Bildirim/YildizGuncelle', { id: bildirimId }, function (response) {
    if (response.success) {
      btn.toggleClass('text-warning text-secondary');
    }
  });
});

// Bildirimleri temizle
$('#bildirimleriTemizle').click(function (e) {
  e.stopPropagation();
  if (confirm('Yıldızlı olmayan tüm bildirimleri silmek istediğinize emin misiniz?')) {
    $.post('/Bildirim/Temizle', function (response) {
      if (response.success) {
        bildirimleriYukle();
      }
    });
  }
});

// Sayfa yüklendiğinde ve her 5 dakikada bir bildirimleri güncelle
$(document).ready(function () {
  bildirimleriYukle();
  setInterval(bildirimleriYukle, 300000); // 5 dakika
});

// Tutar input kontrolü
$('.tutar-input').on('input', function () {
  // Sadece sayı ve virgül girişine izin ver
  let value = this.value.replace(/[^\d,]/g, '');

  // Virgül kontrolü
  if (value.includes(',')) {
    let [lira, kurus] = value.split(',');
    // Kuruş kısmını maksimum 2 basamakla sınırla
    if (kurus && kurus.length > 2) {
      kurus = kurus.slice(0, 2);
    }
    value = lira + ',' + (kurus || '');
  }

  this.value = value;
});

// Input'a tıklandığında içeriği temizle
$('.tutar-input, input[type="number"]').on('focus', function () {
  if (this.value === '0' || this.value === '0,00') {
    this.value = '';
  }
});

// Input boş bırakılırsa varsayılan değeri geri yükle
$('input[type="number"]').on('blur', function () {
  if (!this.value) {
    this.value = '0';
  }
});

// Input'tan çıkıldığında format kontrolü
$('.tutar-input').on('blur', function () {
  if (this.value) {
    // Sadece virgül girilmişse başına 0 ekle
    if (this.value === ',') {
      this.value = '0,';
    }
    // Virgül varsa ve tek basamak girilmişse sıfır ekle
    if (this.value.includes(',') && this.value.split(',')[1].length === 1) {
      this.value = this.value + '0';
    }
    // Virgül yoksa ,00 ekle
    if (!this.value.includes(',')) {
      this.value = this.value + ',00';
    }
  } else {
    this.value = '0,00';
  }
});

function formatTutar(input) {
  let value = input.value;

  // Sadece sayı ve virgüle izin ver
  value = value.replace(/[^0-9,]/g, "");

  // Virgülden sonra en fazla 2 basamak kontrolü
  const parts = value.split(",");
  if (parts.length > 1) {
    parts[1] = parts[1].substring(0, 2); // İlk iki karakteri al
    value = parts.join(",");
  }

  // Değeri güncelle
  input.value = value;

  // Input'tan çıkıldığında format kontrolü
  input.onblur = function () {
    if (this.value) {
      // Sadece virgül girilmişse başına 0 ekle
      if (this.value === ',') {
        this.value = '0,';
      }
      // Virgül varsa ve tek basamak girilmişse sıfır ekle
      if (this.value.includes(',') && this.value.split(',')[1].length === 1) {
        this.value = this.value + '0';
      }
      // Virgül yoksa ,00 ekle
      if (!this.value.includes(',')) {
        this.value = this.value + ',00';
      }
    } else {
      this.value = '0,00';
    }
  };
}
