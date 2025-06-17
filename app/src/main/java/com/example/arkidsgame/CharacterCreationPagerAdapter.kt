package com.example.arkidsgame

import androidx.fragment.app.Fragment
import androidx.fragment.app.FragmentActivity
import androidx.viewpager2.adapter.FragmentStateAdapter
import com.example.arkidsgame.fragments.AvatarFragment
import com.example.arkidsgame.fragments.CertificateFragment
import com.example.arkidsgame.fragments.EquipmentFragment
import com.example.arkidsgame.fragments.VehicleFragment

class CharacterCreationPagerAdapter(activity: FragmentActivity) : FragmentStateAdapter(activity) {
    
    override fun getItemCount(): Int = 4
    
    override fun createFragment(position: Int): Fragment {
        return when (position) {
            0 -> AvatarFragment()
            1 -> EquipmentFragment()
            2 -> VehicleFragment()
            3 -> CertificateFragment()
            else -> AvatarFragment()
        }
    }
} 