package com.example.arkidsgame

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.Button
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.viewpager2.widget.ViewPager2
import com.example.arkidsgame.fragments.AvatarFragment
import com.example.arkidsgame.fragments.CertificateFragment
import com.example.arkidsgame.fragments.EquipmentFragment
import com.example.arkidsgame.fragments.VehicleFragment
import com.google.android.material.tabs.TabLayout
import com.google.android.material.tabs.TabLayoutMediator

class CharacterCreationActivity : AppCompatActivity() {

    private lateinit var viewPager: ViewPager2
    private lateinit var tabLayout: TabLayout
    private lateinit var nextButton: Button
    private lateinit var backButton: Button
    private lateinit var arButton: Button
    private lateinit var stepTextView: TextView
    
    private val TAG = "CharacterCreationActivity"
    
    // Karakter özellikleri
    private var hairStyle = 0
    private var eyeColor = 0
    private var clothes = 0
    private var accessory = 0
    private var equipment = 0
    private var vehicle = 0
    private var explorerName = ""
    
    // Adım başlıkları
    private val stepTitles = listOf(
        "1. Avatar Oluştur",
        "2. Ekipman Seç",
        "3. Araç Seç",
        "4. Sertifikanı Al"
    )
    
    // Tab başlıkları
    private val tabTitles = listOf("Avatar", "Ekipman", "Araç", "Sertifika")
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        try {
            setContentView(R.layout.activity_character_creation)
            
            // UI elemanlarını tanımla
            try {
                viewPager = findViewById(R.id.viewPager)
                tabLayout = findViewById(R.id.tabLayout)
                nextButton = findViewById(R.id.nextButton)
                backButton = findViewById(R.id.backButton)
                arButton = findViewById(R.id.arButton)
                stepTextView = findViewById(R.id.stepTextView)
            } catch (e: Exception) {
                Log.e(TAG, "UI elemanlarını tanımlarken hata: ${e.message}", e)
                Toast.makeText(this, "UI elemanlarını tanımlarken hata: ${e.message}", Toast.LENGTH_LONG).show()
                return
            }
            
            try {
                // ViewPager adaptörünü ayarla
                val adapter = CharacterCreationPagerAdapter(this)
                viewPager.adapter = adapter
                
                // TabLayout ile ViewPager2'yi bağla
                TabLayoutMediator(tabLayout, viewPager) { tab, position ->
                    tab.text = tabTitles[position]
                }.attach()
            } catch (e: Exception) {
                Log.e(TAG, "ViewPager ve TabLayout ayarlanırken hata: ${e.message}", e)
                Toast.makeText(this, "ViewPager ve TabLayout ayarlanırken hata: ${e.message}", Toast.LENGTH_LONG).show()
            }
            
            // İlk adımın başlığını göster
            stepTextView.text = stepTitles[0]
            
            // Geri butonu ilk sayfada gizli olsun
            backButton.visibility = View.INVISIBLE
            
            // Buton tıklama işlevlerini ayarla
            setupButtonListeners()
            
            try {
                // Sayfa değişim dinleyicisi
                viewPager.registerOnPageChangeCallback(object : ViewPager2.OnPageChangeCallback() {
                    override fun onPageSelected(position: Int) {
                        // Adım başlığını güncelle
                        stepTextView.text = stepTitles[position]
                        
                        // İlk sayfada geri butonu gizli olsun
                        backButton.visibility = if (position == 0) View.INVISIBLE else View.VISIBLE
                        
                        // Son sayfada ileri butonu "Tamamla" olsun
                        nextButton.text = if (position == viewPager.adapter?.itemCount?.minus(1) ?: 0) "Tamamla" else "İleri"
                        
                        // AR butonunu göster/gizle (ilk sayfada göster)
                        arButton.visibility = if (position == 0) View.VISIBLE else View.GONE
                    }
                })
            } catch (e: Exception) {
                Log.e(TAG, "Sayfa değişim dinleyicisi ayarlanırken hata: ${e.message}", e)
            }
        } catch (e: Exception) {
            Log.e(TAG, "onCreate hatası: ${e.message}", e)
            Toast.makeText(this, "Uygulama başlatılırken hata oluştu: ${e.message}", Toast.LENGTH_LONG).show()
        }
    }
    
    private fun setupButtonListeners() {
        try {
            nextButton.setOnClickListener {
                val currentItem = viewPager.currentItem
                
                if (currentItem < (viewPager.adapter?.itemCount ?: 0) - 1) {
                    // Sonraki sayfaya geç
                    viewPager.currentItem = currentItem + 1
                } else {
                    // Son sayfadayız, karakter oluşturmayı tamamla
                    finishCharacterCreation()
                }
            }
            
            backButton.setOnClickListener {
                val currentItem = viewPager.currentItem
                
                if (currentItem > 0) {
                    // Önceki sayfaya geç
                    viewPager.currentItem = currentItem - 1
                }
            }
            
            arButton.setOnClickListener {
                // AR görünümüne geç
                startARMode()
            }
        } catch (e: Exception) {
            Log.e(TAG, "Buton işlevlerini ayarlarken hata: ${e.message}", e)
            Toast.makeText(this, "Buton işlevlerini ayarlarken hata: ${e.message}", Toast.LENGTH_LONG).show()
        }
    }
    
    private fun startARMode() {
        try {
            // AR aktivitesine geç
            val intent = Intent(this, ARActivity::class.java)
            
            // Karakter özelliklerini intent'e ekle
            intent.putExtra("HAIR_STYLE", hairStyle)
            intent.putExtra("EYE_COLOR", eyeColor)
            intent.putExtra("CLOTHES", clothes)
            intent.putExtra("ACCESSORY", accessory)
            intent.putExtra("EQUIPMENT", equipment)
            intent.putExtra("VEHICLE", vehicle)
            intent.putExtra("EXPLORER_NAME", explorerName)
            intent.putExtra("START_AR_MODE", true) // AR modunu otomatik başlat
            
            startActivity(intent)
            
            // AR deneyimi için bilgi göster
            Toast.makeText(this, "AR modunda karakterini deneyebilirsin!", Toast.LENGTH_LONG).show()
        } catch (e: Exception) {
            Log.e(TAG, "AR modunu başlatırken hata: ${e.message}", e)
            Toast.makeText(this, "AR modunu başlatırken hata: ${e.message}", Toast.LENGTH_LONG).show()
        }
    }
    
    private fun finishCharacterCreation() {
        try {
            // Karakter oluşturma tamamlandı, ana oyun ekranına geç
            val intent = Intent(this, MainActivity::class.java)
            
            // Karakter özelliklerini intent'e ekle
            intent.putExtra("HAIR_STYLE", hairStyle)
            intent.putExtra("EYE_COLOR", eyeColor)
            intent.putExtra("CLOTHES", clothes)
            intent.putExtra("ACCESSORY", accessory)
            intent.putExtra("EQUIPMENT", equipment)
            intent.putExtra("VEHICLE", vehicle)
            intent.putExtra("EXPLORER_NAME", explorerName)
            
            intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP or Intent.FLAG_ACTIVITY_NEW_TASK)
            startActivity(intent)
            finish()
        } catch (e: Exception) {
            Log.e(TAG, "Karakter oluşturmayı tamamlarken hata: ${e.message}", e)
            Toast.makeText(this, "Karakter oluşturmayı tamamlarken hata: ${e.message}", Toast.LENGTH_LONG).show()
        }
    }
    
    // Fragment'lardan çağrılacak güncelleme metodları
    fun updateHairStyle(index: Int) {
        hairStyle = index
    }
    
    fun updateEyeColor(index: Int) {
        eyeColor = index
    }
    
    fun updateClothes(index: Int) {
        clothes = index
    }
    
    fun updateAccessory(index: Int) {
        accessory = index
    }
    
    fun updateEquipment(index: Int) {
        equipment = index
    }
    
    fun updateVehicle(index: Int) {
        vehicle = index
    }
    
    fun updateExplorerName(name: String) {
        explorerName = name
    }
    
    fun completeCertificate() {
        // Sertifika tamamlandı, karakter oluşturmayı bitir
        finishCharacterCreation()
    }
} 